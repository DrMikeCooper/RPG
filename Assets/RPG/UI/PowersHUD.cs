using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class PowersHUD : MonoBehaviour, IDragAndDropContainer
    {
        public Character character;
        public GameObject icon;
        MenuItem[] items;

        // Use this for initialization
        void Start()
        {
            // find the Icon for each power and clone one
            icon = transform.Find("Icon").gameObject;
            

            Init();
        }

        public void SetCharacter(Character ch)
        {
            character = ch;
            Init();
        }

        void Init()
        {
            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                    ObjectFactory.Recycle(items[i].gameObject);
            }

            RectTransform iconRect = icon.GetComponent<RectTransform>();

            items = new MenuItem[character.powers.Length];
            for (int i = 0; i < character.powers.Length; i++)
            {
                GameObject obj = ObjectFactory.GetObject(icon);
                obj.SetActive(true);
                obj.transform.parent = transform;
                obj.name = "Icon_" + character.powers[i].name;
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.position = iconRect.position + 48 * i * Vector3.right;
                items[i] = obj.GetComponent<MenuItem>();
                items[i].Init(character, null, character.powers[i], i);
                IDraggable draggable = items[i].GetComponentInChildren<IDraggable>();
                if (draggable)
                    draggable.index = i;
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

        public bool CanDrag(IDraggable obj)
        {
            return true;
        }

        public bool CanDrop(IDraggable dragged, IDraggable drop)
        {
            return dragged.GetContainer() == drop.GetContainer();
        }

        public void Drop(IDraggable dragged, IDraggable drop, int replacedIndex)
        {
            // swap powers within our own character's list
            Power p = character.powers[replacedIndex];
            character.powers[replacedIndex] = character.powers[dragged.index];
            character.powers[dragged.index] = p;

            MenuItem it = items[replacedIndex];
            items[replacedIndex] = items[dragged.index];
            items[dragged.index] = it;

            items[replacedIndex].Init(character, null, character.powers[replacedIndex], replacedIndex);
            items[dragged.index].Init(character, null, character.powers[dragged.index], dragged.index);
        }
    }
}