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
            if (fx)
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

        public override void UpdateAction(AIBrain brain)
        {
            // activate the power now, our evaluate has told us its a good idea!
            OnDoPower(brain);
        }

        public override float Evaluate(AIBrain brain, AINode.AICondition condition)
        {
            // TODO
            return GetSplashDamage(brain.character) * 0.5f *(maxDamage + minDamage);
        }

        // AI Utility
        float GetSplashDamage(Character caster)
        {
            float total = 0;
            foreach (Character ch in getAll())
            {
                if (Vector3.Distance(caster.transform.position, ch.transform.position) < radius)
                    if (ch != caster && ch.team != caster.team)
                        total += 1.0f;
                    else
                        total -= 1.0f;
            }
            return total;
        }
    }
}
