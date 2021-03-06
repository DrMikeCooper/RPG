﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerProjectile", menuName = "RPG/Powers/PowerProjectile", order = 1)]
    public class PowerProjectile : Power
    {
        [Header("Power Projectile Settings")]
        public float speed = 1;
        public VisualEffect projectileFX;
        public int maxChains = 0;

        PowerProjectile()
        {
            range = 20;
            speed = 1;
            type = RPG.RPGSettings.DamageType.Piercing;
            targetType = RPG.Power.TargetType.Enemies;
            mode = RPG.Power.Mode.Instant;
            tint.code = RPG.RPGSettings.ColorCode.Piercing;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerProjectile Icon.png");
                if (projectileFX == null)
                    projectileFX = UnityEditor.AssetDatabase.LoadAssetAtPath<RPG.VisualEffect>("Assets/Example Particles/Trail.asset");
            }
#endif
        }

        public override void OnActivate(Character caster, bool doStatus = true)
        {
            caster.FaceTarget();

            // spawn a projectile and set it going in the right direction
            GameObject go = ObjectFactory.GetObject(RPGSettings.instance.projectile);
            go.transform.position = caster.GetBodyPart(userBodyPart).position;
            Projectile proj = go.GetComponent<Projectile>();
            Vector3 velocity = caster.transform.forward * speed;
            proj.Init(this, caster, velocity, doStatus);

            // create particles on the projectile
            GameObject pgo = projectileFX.Begin(proj.transform, tint, false, false);
            pgo.transform.parent = go.transform;
            pgo.transform.localPosition = Vector3.zero;

            // TODO - pass doStatus on to projectile
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
