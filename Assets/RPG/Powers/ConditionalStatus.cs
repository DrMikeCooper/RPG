using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class ConditionalStatus : Status
    {
        // the condition to check against the caster before using
        [Header("Activation condition")]
        [Tooltip("Condition that must be met to apply this status")]
        public AINode.AICondition condition;
        public Status status;

        public override void Apply(Prop ch, Character caster = null)
        {
            Power p = null;
            if (caster)
                p = caster.activePower;

            if (AINode.IsCondition(condition, ch, p))
                status.Apply(ch, caster);
        }

        public override string GetDescription(bool brief = false)
        {
            return status.GetDescription(brief);
        }
    }
}