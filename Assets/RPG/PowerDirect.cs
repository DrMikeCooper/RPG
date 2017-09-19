﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerDirect", menuName = "RPG/PowerDirect", order = 2)]
    public class PowerDirect : Power
    {
        [Header("Power Direct Settings")]
        public Material beamMaterial;
        [ShowIf("beamMaterial")]
        public float beamWidth = 0.5f;
        [ShowIf("beamMaterial")]
        public float beamUVSpeed = 1.0f;
        [ShowIf("beamMaterial")]
        public int maxChains = 0;

        // useable on a single target with no to-hit roll
        public override void OnActivate(Character caster)
        {
            Prop target = GetTarget(caster);

            if (target)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                Character ctarget = target as Character;
                if (ctarget)
                    ctarget.MakeAwareOf(caster);

                // draw the beam to the target
                if (beamMaterial)
                {
                    caster.beam.Activate(caster.GetBodyPart(userBodyPart), target.GetBodyPart(targetBodyPart), 1.0f, beamMaterial, beamWidth, tint.GetColor());
                    caster.beam.uvSpeed = beamUVSpeed;
                }

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
                        if (!all[i].dead && all[i].team != ctarget.team)
                            targets.Add(all[i]);
                    }

                    Arc(ctarget, targets, charge);
                    return;
                }

                // no deflection, carry on...
                bool hit = Apply(target, charge, caster);

                // if we're a beam, check for chaining
                if (beamMaterial)
                {
                    // chained beams
                    if (maxChains > 0)
                    {
                        // get all targets except the primary one on the other team
                        List<Character> targets = new List<Character>();
                        Character[] all = PowerArea.getAll();
                        for (int i = 0; i < all.Length; i++)
                        {
                            if (all[i] != ctarget && all[i].team == ctarget.team)
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
        }

        public override float Evaluate(AIBrain brain, AINode.AICondition condition)
        {
            return EvaluateRanged(brain, condition);
        }

        public override void UpdateAction(AIBrain brain)
        {
            OnUpdateRanged(brain);
        }

        Character Arc(Character lastTarget, List<Character> targets, float charge)
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
                GameObject beamObj = RPGSettings.instance.beamPool.GetObject();
                if (beamObj)
                {
                    BeamRenderer beam = beamObj.GetComponent<BeamRenderer>();
                    beam.Activate(lastTarget.GetBodyPart(targetBodyPart), newTarget.GetBodyPart(targetBodyPart), 1.0f, beamMaterial, beamWidth, tint.GetColor());
                    beam.uvSpeed = beamUVSpeed;
                }
                else
                    Debug.Log("Running out of beams! Increase the pool size in RPGSettings");

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