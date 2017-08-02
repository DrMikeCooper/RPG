using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerDirect", menuName = "RPG/PowerDirect", order = 1)]
    public class PowerDirect : Power
    {
        public Material beamMaterial;
        public float beamWidth = 0.5f;
        public float beamUVSpeed = 1.0f;

        // useable on a single target with no to-hit roll
        public override void OnActivate(Character caster)
        {
            // check standard cooldown, energy cost etc
            if (!CanUse(caster))
                return;

            Character target = GetTarget(caster);

            if (target)
            {
                UsePower(caster);
                Apply(target);
                if (beamMaterial)
                {
                    caster.beam.Activate(caster.GetBodyPart(userBodyPart), target.GetBodyPart(targetBodyPart), 1.0f, beamMaterial, beamWidth, RPGSettings.GetColor(color));
                    caster.beam.uvSpeed = beamUVSpeed;
                }
            }
        }
    }
}