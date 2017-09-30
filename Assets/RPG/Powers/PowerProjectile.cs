using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerProjectile", menuName = "RPG/PowerProjectile", order = 2)]
    public class PowerProjectile : Power
    {
        [Header("Power Projectile Settings")]
        public float speed = 1;
        public VisualEffect projectileFX;
        public int maxChains = 0;

        public override void OnActivate(Character caster)
        {
            // spawn a projectile and set it going in the right direction
            GameObject go = Instantiate(RPGSettings.instance.projectile);
            go.transform.position = caster.GetBodyPart(userBodyPart).position;
            Projectile proj = go.GetComponent<Projectile>();
            Vector3 velocity = caster.transform.forward * speed;
            proj.Init(this, caster, velocity);

            // create particles on the projectile
            GameObject pgo = projectileFX.Begin(proj.transform, tint, false);
            pgo.transform.parent = go.transform;
            pgo.transform.localPosition = Vector3.zero;
        }

        public override float Evaluate(AIBrain brain, AINode.AICondition condition)
        {
            return EvaluateRanged(brain, condition);
        }

        public override void UpdateAction(AIBrain brain)
        {
            OnUpdateRanged(brain);
        }
    }
}
