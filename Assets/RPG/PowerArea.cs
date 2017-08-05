using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerArea", menuName = "RPG/PowerArea", order = 2)]
    public class PowerArea : Power
    {
        public float radius;
        [Tooltip("Optional settings to allow for different chargeup particles. Leave empty to use userParticles for the explosion")]
        public ParticleSystem explodeParticles;
        public Character.BodyPart explodeBodyPart = Character.BodyPart.Root;

        public override void OnActivate(Character caster)
        {
            Explode(caster.GetBodyPart(explodeBodyPart), caster);
        }

        public void Explode(Transform centre, Character caster)
        {
            AddParticles(explodeParticles == null ? userParticles : explodeParticles, centre);
            float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
            // check all other characters within the radius
            foreach (Character ch in getAll())
            {
                if (ch != caster && Vector3.Distance(ch.transform.position, centre.position) < radius)
                {
                    Apply(ch, charge);
                }
            }
        }
    }
}
