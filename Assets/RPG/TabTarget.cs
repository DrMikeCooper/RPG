﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class TabTarget : MonoBehaviour
    {
        Character[] targets;
        public CharacterHUD hud;
        int index = -1;
        public GameObject reticle;

        // Use this for initialization
        void Start()
        {
            targets = GameObject.FindObjectsOfType<Character>();
            reticle.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                index++;
                if (index >= targets.Length)
                    index = 0;
                hud.SetCharacter(GetTarget());
                reticle.SetActive(true);
            }

            if (index > -1)
            {
                GameObject target = GetTarget().gameObject;
                reticle.transform.position = target.transform.position;
            }

            // left mouse clicks select a character
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit info;
                if (Physics.Raycast(ray, out info))
                {
                    Character ch = info.collider.GetComponent<Character>();
                    index = GetIndex(ch);
                    hud.SetCharacter(ch);
                    reticle.SetActive(index != -1);
                }
            }
        }

        public Character GetTarget()
        {
            if (index < 0)
                return null;
            return targets[index];
        }

        int GetIndex(Character ch)
        {
            for (int i = 0; i < targets.Length; i++)
                if (ch == targets[i])
                    return i;
            return -1;
        }
    }
}
