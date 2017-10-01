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
        MenuItem[] items;

        // Use this for initialization
        void Start()
        {
            // find the Icon for each power and clone one
            icon = transform.Find("Icon").gameObject;
            RectTransform iconRect = icon.GetComponent<RectTransform>();

            items = new MenuItem[character.powers.Length];
            for (int i = 0; i < character.powers.Length; i++)
            {
                GameObject obj = Instantiate(icon);
                obj.SetActive(true);
                obj.transform.parent = transform;
                obj.name = "Icon_" + character.powers[i].name;
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.position = iconRect.position + 48 * i * Vector3.right;
                items[i] = obj.GetComponent<MenuItem>();
                items[i].Init(character, null, character.powers[i], i);
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
                    items[i].SetPower(subP);
                items[i].SetTarget(character.target);
            }
        }
    }
}