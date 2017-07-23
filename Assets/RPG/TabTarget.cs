using System.Collections;
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
        }

        public Character GetTarget()
        {
            if (index < 0)
                return null;
            return targets[index];
        }
    }
}
