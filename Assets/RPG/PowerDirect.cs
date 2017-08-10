using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerDirect", menuName = "RPG/PowerDirect", order = 2)]
    public class PowerDirect : Power
    {
        public Material beamMaterial;
        public float beamWidth = 0.5f;
        public float beamUVSpeed = 1.0f;

        // useable on a single target with no to-hit roll
        public override void OnActivate(Character caster)
        {
            Character target = GetTarget(caster);

            if (target)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                Apply(target, charge);
                if (beamMaterial)
                {
                    caster.beam.Activate(caster.GetBodyPart(userBodyPart), target.GetBodyPart(targetBodyPart), 1.0f, beamMaterial, beamWidth, RPGSettings.GetColor(color));
                    caster.beam.uvSpeed = beamUVSpeed;
                }
            }
        }

        public override float Evaluate(NPCPowers npc)
        {
            Character caster = npc.character;

            npcTarget = null;
            float bestEval = 0;
            foreach (Character ch in getAll())
            {
                if (ch != caster && ch.team != caster.team)
                {
                    float eval = 100.0f / timeToDeath(caster, ch);
                    if (eval > bestEval)
                    {
                        bestEval = eval;
                        npcTarget = ch;
                    }
                }
            }

            return bestEval;
        }

        public override void UpdateAction(NPCPowers npc)
        {
            Character caster = npc.character;
            Character target = npc.target;
            caster.target = target;

            float distance = target ? Vector3.Distance(caster.transform.position, target.transform.position) : 100000;
            if (distance > range)
            {
                npc.MoveTo(target.transform);
                npc.timer = 0.5f;
            }
            else
            {
                npc.MoveTo(caster.transform);
                OnStart(caster);

                // instant powers release the key immediately. Others, we let energy or duration end it.
                if (mode == Mode.Instant)
                    OnEnd(caster);

                npc.timer = 3 + duration;
            }
        }
    }
}