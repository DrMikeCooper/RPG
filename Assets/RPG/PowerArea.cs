using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerDirect", menuName = "RPG/PowerArea", order = 1)]
    public class PowerArea : Power
    {
        public float radius;

        // utility stuff for storing all characters. TODO Review this.
        static Character[] allCharacters;
        static Character[] getAll()
        {
            if (allCharacters == null)
                allCharacters = FindObjectsOfType<Character>();
            return allCharacters;
        }

        public override void OnActivate(Character caster)
        {
            // check standard cooldown, energy cost etc
            if (!CanUse(caster))
                return;

            // energy and cooldowns
            caster.UsePower(this);

            // check all other characters within the radius
            foreach (Character ch in getAll())
            {
                if (ch != caster && Vector3.Distance(ch.transform.position, caster.transform.position) < radius)
                {
                    Apply(ch);
                }
            }
        }
    }
}
