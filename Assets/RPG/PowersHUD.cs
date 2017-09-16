using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class PowersHUD : MonoBehaviour
    {
        public Character character;
        public GameObject icon;
        public Image[] icons;

        // Use this for initialization
        void Start()
        {
            // find the Icon for each power and clone one
            icon = transform.Find("Icon").gameObject;
            RectTransform iconRect = icon.GetComponent<RectTransform>();

            icons = new Image[character.powers.Length];
            for (int i = 0; i < character.powers.Length; i++)
            {
                GameObject obj = Instantiate(icon);
                obj.SetActive(true);
                obj.transform.parent = transform;
                obj.name = "Icon_" + character.powers[i].name;
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.position = iconRect.position + 48 * i * Vector3.right;
                icons[i] = obj.GetComponent<Image>();
                icons[i].sprite = character.powers[i].GetPower(character).icon;
                if (character.powers[i].coolDown > 0)
                {
                    icons[i].type = Image.Type.Filled;
                    icons[i].fillMethod = Image.FillMethod.Radial360;
                }
                ToolTipIcon toolTip = icons[i].GetComponent<ToolTipIcon>();
                if (toolTip)
                {
                    Text text = toolTip.toolTip.GetComponentInChildren<Text>();
                    if (text)
                        text.text = character.powers[i].GetDescription();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < character.powers.Length; i++)
            {
                Power p = character.powers[i];
                Power subP = p.GetPower(character);
                if (p != subP)
                    icons[i].sprite = subP.icon;
                icons[i].color = subP.tint.GetColor();
                if (!subP.CanUse(character) || subP.GetTarget(character) == null)
                {
                    Color col = icons[i].color;
                    col *= 0.5f; col.a = 1;
                    icons[i].color = col;
                }
                icons[i].fillAmount = character.GetCoolDownFactor(character.powers[i]);
                PowerToggle toggle = p as PowerToggle;
                if (toggle && character.toggles.ContainsKey(toggle))
                {
                    icons[i].rectTransform.localScale = Vector3.one * (character.toggles[toggle].on ? 0.7f : 1.0f);
                }
            }
        }
    }
}