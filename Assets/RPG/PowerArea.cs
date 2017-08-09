using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerArea", menuName = "RPG/PowerArea", order = 2)]
    public class PowerArea : Power
    {
        public float radius;
        [Tooltip("Optional settings to allow for different chargeup particles. Leave empty to use userFX for the explosion")]
        public VisualEffect explodeFX;
        public Character.BodyPart explodeBodyPart = Character.BodyPart.Root;

        public override void OnActivate(Character caster)
        {
            Explode(caster.GetBodyPart(explodeBodyPart), caster);
        }

        public void Explode(Transform centre, Character caster)
        {
            VisualEffect fx = explodeFX ? explodeFX : userFX;
            fx.Begin(centre, color);
            float charge = caster ? caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f : 1.0f;
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
