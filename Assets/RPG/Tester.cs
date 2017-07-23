using UnityEngine;
using System.Collections;

namespace RPG
{
    public class Tester : MonoBehaviour {

        Character character;
        TabTarget targetter;

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
                character.ApplyDamage(10, Attack.DamageType.Crushing);
            if (Input.GetKeyDown(KeyCode.U))
                character.ApplyDamage(10, Attack.DamageType.Fire);

            // apply crushing damage resistance buff
            if (Input.GetKeyDown(KeyCode.I))
                character.ApplyStatus("Armour");

            // apply fire resistance debuff

            if (Input.GetKeyDown(KeyCode.O))
                character.ApplyStatus("Bubbles");
            if (Input.GetKeyDown(KeyCode.L))
                character.ApplyStatus("Regen");

            if (Input.GetKeyDown(KeyCode.P))
                character.UseEnergy(50);
        }
    }
}

