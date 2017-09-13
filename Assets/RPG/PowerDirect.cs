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
            Prop target = GetTarget(caster);

            if (target)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                Apply(target, charge, caster);
                Character ctarget = target as Character;
                if (ctarget)
                    ctarget.MakeAwareOf(caster);
                if (beamMaterial)
                {
                    caster.beam.Activate(caster.GetBodyPart(userBodyPart), target.GetBodyPart(targetBodyPart), 1.0f, beamMaterial, beamWidth, RPGSettings.GetColor(color));
                    caster.beam.uvSpeed = beamUVSpeed;
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
    }
}