using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerArea", menuName = "RPG/PowerArea", order = 2)]
    public class PowerArea : Power
    {
        [Header("Power Area Settings")]
        [Tooltip("Distance from caster at which the power stops working")]
        public float radius;
        [Tooltip("Total arc of the power in radians. 360 means circular area of effect")]
        public int angle = 360;

        [Tooltip("Optional settings to allow for different chargeup particles. Leave empty to use userFX for the explosion")]
        public VisualEffect explodeFX;
        public Character.BodyPart explodeBodyPart = Character.BodyPart.Root;

        public override void OnActivate(Character caster)
        {
            if (angle < 360 && caster.target)
                caster.FaceTarget();
            Explode(caster.GetBodyPart(explodeBodyPart), caster);
        }

        public void Explode(Transform centre, Character caster)
        {
            VisualEffect fx = explodeFX ? explodeFX : userFX;
            if (fx)
                fx.Begin(centre, tint);
            float charge = caster ? caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f : 1.0f;

            List<Character> targets = GetTargets(caster, centre.position, centre.forward);

            // check all other characters within the radius
            foreach (Character ch in targets)
                  Apply(ch, charge, caster);
        }

        public List<Character> GetTargets(Character caster, Vector3 centre, Vector3 forward)
        {
            List<Character> targets = new List<Character>();

            float dotMin = Mathf.Cos(angle * Mathf.Deg2Rad * 0.5f);

            foreach (Character ch in getAll())
            {
                if (!ch.dead && ch != caster && Vector3.Distance(ch.transform.position, centre) < radius)
                {
                    // arc test, to see how forwards of caster we are
                    if (angle >= 360 || Vector3.Dot((ch.transform.position - centre).normalized, forward) > dotMin)
                        targets.Add(ch);
                }
            }

            return targets;
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
                    if (ch != caster && ch.team != caster.GetTeam())
                        total += 1.0f;
                    else
                        total -= 1.0f;
            }
            return total;
        }
    }
}
