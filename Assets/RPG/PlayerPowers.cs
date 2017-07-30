using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class PlayerPowers : MonoBehaviour
    {
        Character character;

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
        }

        // Update is called once per frame
        void Update()
        {
            KeyCode key = KeyCode.Alpha1;
            foreach (Power p in character.powers)
            {
                if (Input.GetKeyDown(key))
                    p.OnActivate(character);
                key++;
            }
        }
    }
}