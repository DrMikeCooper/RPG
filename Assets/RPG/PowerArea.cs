using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerArea", menuName = "RPG/PowerArea", order = 2)]
    public class PowerArea : Power
    {
        public float radius;

        public override void OnActivate(Character caster)
        {
            // check standard cooldown, energy cost etc
            if (!CanUse(caster))
                return;

            // energy and cooldowns
            UsePower(caster);

            Explode(caster.transform, caster);
        }

        public void Explode(Transform centre, Character caster)
        {
            // check all other characters within the radius
            foreach (Character ch in getAll())
            {
                if (ch != caster && Vector3.Distance(ch.transform.position, centre.position) < radius)
                {
                    Apply(ch);
                }
            }
        }
    }
}
