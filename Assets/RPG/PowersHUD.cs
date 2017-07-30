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
                icons[i].sprite = character.powers[i].icon;
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < character.powers.Length; i++)
            {
                icons[i].color = character.powers[i].color;
                if (!character.powers[i].CanUse(character) || character.powers[i].GetTarget(character) == null)
                {
                    icons[i].color *= 0.5f;
                }

            }
        }
    }
}