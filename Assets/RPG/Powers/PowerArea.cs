using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerArea", menuName = "RPG/Powers/PowerArea", order = 1)]
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

        [Header("Beam Settings")]
        [Tooltip("Optional beam to track from the user to each target")]
        public PowerBeam.BeamSettings settings = new PowerBeam.BeamSettings(null);

        PowerArea()
        {
            range = 0;
            radius = 5;
            type = RPG.RPGSettings.DamageType.Fire;
            targetType = RPG.Power.TargetType.Enemies;
            mode = RPG.Power.Mode.Instant;
            tint.code = RPG.RPGSettings.ColorCode.Fire;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerArea Icon.png");
            }
#endif
        }

        public override void OnActivate(Character caster, bool doStatus = true)
        {
            if (angle < 360 && caster.target)
                caster.FaceTarget();
            Explode(caster.GetBodyPart(explodeBodyPart), caster, doStatus);
        }

        public void Explode(Transform centre, Character caster, bool doStatus = true)
        {
            VisualEffect fx = explodeFX;
            if (fx)
            {
                GameObject go = fx.Begin(centre, tint);
                fx.ScaleToRadius(go, GetRadius(caster));
            }
            float charge = caster ? caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f : 1.0f;

            List<Character> targets = GetTargets(caster, centre.position, centre.forward);

            // check all other characters within the radius
            foreach (Character ch in targets)
            {
                Apply(ch, charge, caster, doStatus);

                // add a beam to each one
                if (settings.beamMaterial != null || settings.beamParticles != null)
                {
                    AddBeamBetween(caster, ch, userBodyPart, settings);
                }
            }
        }

        public List<Character> GetTargets(Character caster, Vector3 centre, Vector3 forward)
        {
            List<Character> targets = new List<Character>();

            float dotMin = Mathf.Cos(angle * Mathf.Deg2Rad * 0.5f);

            float rad = GetRadius(caster);
            foreach (Character ch in getAll())
            {
                if (!ch.dead && ch != caster && Vector3.Distance(ch.transform.position, centre) < rad)
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
            float r = GetRadius(caster);
            foreach (Character ch in getAll())
            {
                if (Vector3.Distance(caster.transform.position, ch.transform.position) < r)
                    if (ch != caster && ch.team != caster.GetTeam())
                        total += 1.0f;
                    else
                        total -= 1.0f;
            }
            return total;
        }

        public float GetRadius(Character caster)
        {
            float r = radius;
            if (maxRadius!=0 && caster)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                r *= charge * maxRadius + (1 - charge) * radius;
            }
            return r;
        }
    }
}
