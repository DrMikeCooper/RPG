using UnityEngine;
using System.Collections;

namespace RPG
{
    public class Tester : MonoBehaviour {

        Character character;

        // Use this for initialization
        void Start() {
            character = GetComponent<Character>();
        }

        // Update is called once per frame
        void Update() {

            // apply fire or crushing damage
            if (Input.GetKeyDown(KeyCode.Y))
                character.ApplyDamage(10, Attack.DamageType.Crushing);
            if (Input.GetKeyDown(KeyCode.U))
                character.ApplyDamage(10, Attack.DamageType.Fire);

            // apply crushing damage resistance buff
            if (Input.GetKeyDown(KeyCode.I))
                character.ApplyStatus(Buff.GetResBuff(Attack.DamageType.Crushing, 100, 5));

            // apply fire resistance debuff

            if (Input.GetKeyDown(KeyCode.O))
                character.ApplyStatus(Buff.GetEnergiseBuff(5, 5));
            if (Input.GetKeyDown(KeyCode.L))
                character.ApplyStatus(Buff.GetRegenBuff(5, 5));

            if (Input.GetKeyDown(KeyCode.P))
                character.UseEnergy(50);
        }
    }
}

