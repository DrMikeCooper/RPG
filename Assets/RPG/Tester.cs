using UnityEngine;
using System.Collections;
using System;

namespace RPG
{
    public class Tester : MonoBehaviour {

        Prop character;
        TabTarget targetter;

        [Serializable]
        public struct TestItem
        {
            public KeyCode key;
            public Status status;
        }

        public TestItem[] tests;
    
        // Use this for initialization
        void Start() {
            character = GetComponent<Character>();
            targetter = GetComponent<TabTarget>();
        }

        // Update is called once per frame
        void Update() {

            character = targetter.GetTarget();

            // apply fire or crushing damage
            if (Input.GetKeyDown(KeyCode.Y))
                character.ApplyDamage(10, RPGSettings.DamageType.Crushing);
            if (Input.GetKeyDown(KeyCode.U))
                character.ApplyDamage(10, RPGSettings.DamageType.Fire);
            Character c = character as Character;
            if (Input.GetKeyDown(KeyCode.P) && c)
                c.UseEnergy(50);

            for (int i = 0; i < tests.Length; i++)
            {
                if (Input.GetKeyDown(tests[i].key))
                    character.ApplyStatus(tests[i].status, 20);
            }
        }
    }
}

