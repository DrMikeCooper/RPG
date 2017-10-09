using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "EnergyDrain", menuName = "RPG/Status/EnergyDrain", order = 1)]
    public class EnergyDrain : Status
    {
        [Header("Energy Drain Settings")]
        public float drain;

        public override void Apply(Prop ch, Character caster = null)
        {
            Character cha = ch as Character;
            if (cha)
                cha.energy -= drain;
        }

        public override string GetDescription(bool brief = false)
        {
            return "Energy Drain (" + drain + ")";
        }

        public override bool isImmediate() { return true; }

        // used for AI evaluation
        public override float DamagePerHit()
        {
            return drain * 0.5f;
        }
    }
}