using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace RPG
{

    public class CharacterHUD : MonoBehaviour
    {
        // the character this displays info for
        public Character character;

        // inner class that represents any kind of meter
        class Bar
        {
            RectTransform meter;
            Rect initialRect;
            string name;

            public Bar(string n) { name = n; }

            public void Init(RectTransform child)
            {
                if (child.name == name)
                {
                    meter = child;
                    initialRect = meter.rect;
                }
            }

            public void Update(float pct)
            {
                if (meter)
                {
                    meter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pct * initialRect.width);
                    Vector3 pos = meter.localPosition;
                    pos.x = 0.5f * (pct-1.0f) * initialRect.width;
                    meter.localPosition = pos;
                }
            }
        };
    
        // sub-components to manipulate
        Bar healthBar = new Bar("HealthBar");
        Bar energyBar = new Bar("EnergyBar");

        // the icon template set up in the editor
        GameObject icon;
        Text characterName;
        Image portrait;

        // current icons
        Dictionary<string, GameObject> icons = new Dictionary<string, GameObject>();

        // Use this for initialization
        void Start()
        {
            RectTransform[] children = GetComponentsInChildren<RectTransform>();
            foreach (RectTransform child in children)
            {
                healthBar.Init(child);
                energyBar.Init(child);
                if (child.name == "Name")
                    characterName = child.GetComponent<Text>();
                if (child.name == "Portrait")
                    portrait = child.GetComponent<Image>();
            }

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
        }

        public void SetCharacter(Character ch)
        {
            if (character)
                character.onStatusChanged.RemoveListener(UpdateIcons);
            character = ch;
            if (character)
            {
                character.onStatusChanged.AddListener(UpdateIcons);
                if (portrait)
                    portrait.sprite = character.portrait;
                if (characterName)
                    characterName.text = character.characterName;
            }

            gameObject.SetActive(character != null);
            UpdateIcons();
        }

        // Update is called once per frame
        void Update()
        {
            if (character != null)
            {
                healthBar.Update(character.GetHealthPct());
                energyBar.Update(character.GetEnergyPct());
            }
        }

        public void UpdateIcons()
        {
            if (icon == null)
                return;

            // delete old gameobjects and clear the list
            foreach (KeyValuePair<string, GameObject> pair in icons)
            {
                Destroy(pair.Value);
            }
            icons.Clear();

            float offsetX = 0;

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
                        rect.localPosition = iconLocalPosition + offsetX * Vector3.right;
                        offsetX += 18; //todo
                        icons[s.name] = ike;
                        ike.name = s.name;

                        // copy the colour and image from global settings
                        ike.GetComponent<Image>().color = RPGSettings.GetColor(s.color);
                        ike.GetComponent<Image>().sprite = s.icon;

                        Transform counter = ike.transform.Find("Counter");
                        if (counter)
                        {
                            counter.gameObject.SetActive(s.count > 1);
                            Text text = counter.GetComponent<Text>();
                            text.text = "" + s.count;
                        }
                    }
                }
            }
        }
        
    }
}
