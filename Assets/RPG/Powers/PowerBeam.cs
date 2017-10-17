using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerBeam", menuName = "RPG/Powers/PowerBeam", order = 1)]
    public class PowerBeam : PowerDirect
    {
        [System.Serializable]
        public struct BeamSettings
        {
            public BeamSettings(Material mat)
            {
                beamMaterial = mat;
                beamWidth = 0.5f;
                beamLength = 1.0f;
                beamUVSpeed = 1.0f;
                beamParticles = null;
                beamDuration = 2;
            }

            public Material beamMaterial;
            [ShowIf("beamMaterial")]
            public float beamWidth;
            [ShowIf("beamMaterial")]
            public float beamLength;
            [ShowIf("beamMaterial")]
            public float beamUVSpeed;
            public BeamParticles beamParticles;
            public float beamDuration;
        }

        [Header("Beam Settings")]
        public BeamSettings beamSettings = new BeamSettings(null);

        public int maxChains = 0;

        PowerBeam()
        {
            range = 20;
            type = RPG.RPGSettings.DamageType.Energy;
            targetType = RPG.Power.TargetType.Enemies;
            mode = RPG.Power.Mode.Instant;
            tint.code = RPG.RPGSettings.ColorCode.Energy;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerBeam Icon.png");
                if (beamSettings.beamMaterial == null)
                    beamSettings.beamMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RadialBeam.mat");
            }
#endif
        }

        // useable on a single target with no to-hit roll
        public override void OnActivate(Character caster, bool doStatus = true)
        {
            Prop target = GetTarget(caster);

            if (target)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                Character ctarget = target as Character;
                if (ctarget)
                    ctarget.MakeAwareOf(caster);

                // draw the beam to the target
                DrawBeam(caster, target);

                // check for deflection shields on the target
                HitResponse.ReflectionType deflect = target.GetReflection(type);
                if (deflect != HitResponse.ReflectionType.None)
                {
                    // get all targets except the primary one on the other team
                    List<Character> targets = new List<Character>();
                    Character[] all = PowerArea.getAll();
                    for (int i = 0; i < all.Length; i++)
                    {
                        // TODO - chaining ally powers
                        if (!all[i].dead && all[i].GetTeam() != ctarget.team)
                            targets.Add(all[i]);
                    }

                    Arc(ctarget, targets, charge);
                    return;
                }

                // no deflection, carry on...
                bool hit = Apply(target, charge, caster, doStatus);

                CheckChaining(ctarget, charge);
            }
        }

        void DrawBeam(Character caster, Prop target)
        {
            if (beamSettings.beamMaterial || beamSettings.beamParticles)
            {
                caster.beam.Activate(caster.GetBodyPart(userBodyPart), target.GetBodyPart(targetBodyPart), beamSettings, tint.GetColor());
            }
        }

        void CheckChaining(Character ctarget, float charge)
        {
            // if we're a beam, check for chaining
            if (beamSettings.beamMaterial || beamSettings.beamParticles)
            {
                // chained beams
                if (maxChains > 0)
                {
                    // get all targets except the primary one on the other team
                    List<Character> targets = new List<Character>();
                    Character[] all = PowerArea.getAll();
                    for (int i = 0; i < all.Length; i++)
                    {
                        if (all[i] != ctarget && all[i].GetTeam() == ctarget.team)
                            targets.Add(all[i]);
                    }

                    for (int i = 0; i < maxChains; i++)
                    {
                        if (ctarget)
                            ctarget = Arc(ctarget, targets, charge);
                    }
                }
            }
        }

        public Character Arc(Character lastTarget, List<Character> targets, float charge)
        {
            List<Character> subTargets = new List<Character>();
            foreach (Character t in targets)
                if (!t.dead && lastTarget.CanSee(t))
                    subTargets.Add(t);

            if (subTargets.Count > 0)
            {
                // pick a random target from the list
                Character newTarget = subTargets[Random.Range(0, subTargets.Count)];

                // create the visible beam
                AddBeamBetween(newTarget, lastTarget, targetBodyPart, beamSettings);

                // Apply the game effects to the next target and alert them
                Apply(newTarget, charge, lastTarget);
                newTarget.MakeAwareOf(lastTarget);

                return newTarget;
            }
            else
                return null;
        }
    }
}
