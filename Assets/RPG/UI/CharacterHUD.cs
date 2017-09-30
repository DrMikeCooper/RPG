using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace RPG
{

    public class CharacterHUD : MonoBehaviour
    {
        // the character this displays info for
        public Prop character;
        public CharacterHUD myTarget;
        public Vector3 iconOffset = new Vector3(18, 0, 0);

        // the icon template set up in the editor
        GameObject icon;
        Text characterName;
        Image portrait;

        // current icons
        Dictionary<string, GameObject> icons = new Dictionary<string, GameObject>();
        CharacterHUDMeter[] meters;

        bool firstUpdate;

        Dictionary<Status, Text> timers = new Dictionary<Status, Text>();

        // Use this for initialization
        void Start()
        {
            RectTransform[] children = GetComponentsInChildren<RectTransform>();
            foreach (RectTransform child in children)
            {
                if (child.name == "Name")
                    characterName = child.GetComponent<Text>();
                if (child.name == "Portrait")
                    portrait = child.GetComponent<Image>();
            }

            meters = transform.GetComponentsInChildren<CharacterHUDMeter>();

            // find a cloneable icon for status effects, and disable it
            Transform iconChild = transform.Find("Icon");
            if (iconChild)
            {
                icon = iconChild.gameObject;
                icon.SetActive(false);
            }

            gameObject.SetActive(character != null);
            if (character)
                character.onStatusChanged.AddListener(UpdateIcons);

            firstUpdate = true;
        }

        public void SetCharacter(Prop ch)
        {
            if (character)
                character.onStatusChanged.RemoveListener(UpdateIcons);
            character = ch;
            if (character)
            {
                character.onStatusChanged.AddListener(UpdateIcons);
                if (portrait)
                {
                    portrait.sprite = character.portrait;
                    portrait.transform.parent.gameObject.SetActive(character.portrait != null);
                }
                if (characterName)
                    characterName.text = character.characterName;

                // turn off any meters that don't make sense for the target, eg no energy for props
                foreach (CharacterHUDMeter meter in meters)
                {
                    if (character.stats != null)
                        meter.transform.parent.gameObject.SetActive(character.stats.ContainsKey(meter.stat.ToString()));
                }
            }
            else
            {
                if (myTarget)
                    myTarget.SetCharacter(null);
            }

            gameObject.SetActive(character != null);
            UpdateIcons(character, null);
        }

        // Update is called once per frame
        void Update()
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                SetCharacter(character);
            }

            if (myTarget != null)
            {
                Prop tgt = character == null ? null : character.target;
                myTarget.gameObject.SetActive(tgt != null); // needed for when a target gets destroyed
                if (myTarget.character != tgt)
                    myTarget.SetCharacter(tgt);
            }

            foreach (var pair in timers)
            {
                pair.Value.text = ((int)(pair.Key.duration - pair.Key.timer)).ToString();
            }
        }

        public void UpdateIcons(Prop p, Status newStatus)
        {
            if (icon == null)
                return;

            timers.Clear();

            // delete old gameobjects and clear the list
            foreach (KeyValuePair<string, GameObject> pair in icons)
            {
                Destroy(pair.Value);
            }
            icons.Clear();

            Vector3 offsetX = Vector3.zero;

            if (character != null)
            {
                Vector3 iconLocalPosition = icon.GetComponent<RectTransform>().localPosition;
                foreach (KeyValuePair<string, Status> pair in character.groupedEffects)
                {
                    Status s = pair.Value;
                    if (s.icon != null)
                    {
                        GameObject ike = Instantiate(icon);
                        ike.SetActive(true);
                        ike.transform.SetParent(transform);
                        RectTransform rect = ike.GetComponent<RectTransform>();
                        rect.localPosition = iconLocalPosition + offsetX;
                        rect.localScale = Vector3.one;
                        offsetX += iconOffset; //todo
                        icons[s.name] = ike;
                        ike.name = s.name;

                        // copy the colour and image from global settings
                        ike.GetComponent<Image>().color = s.tint.GetColor();
                        ike.GetComponent<Image>().sprite = s.icon;

                        Transform counter = ike.transform.Find("Counter");
                        if (counter)
                        {
                            counter.gameObject.SetActive(s.count > 1);
                            Text text = counter.GetComponent<Text>();
                            text.text = "" + s.count;
                        }

                        ToolTipIcon toolTip = ike.GetComponent<ToolTipIcon>();
                        if (toolTip)
                        {
                            Text text = toolTip.toolTip.GetComponentInChildren<Text>();
                            if (text)
                                text.text = s.GetDescription();
                        }
                        if (s.duration < 100000)
                        {
                            Transform timer = ike.transform.Find("Timer");
                            if (timer)
                            {
                                timers[s] = timer.GetComponent<Text>();
                                timers[s].enabled = true;
                            }
                        }
                    }
                }
            }
        }
        
    }
}
