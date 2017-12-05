using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "Conversion", menuName = "RPG/Status/Conversion", order = 2)]
    public class Conversion : Status
    {
        [Header("Conversion Settings")]
        [Tooltip("The status that feees this conversion")]
        public Status source;
        [Tooltip("Does it remove the source status from the target?")]
        public bool consume;
        public Status resultStatus;
        public bool applyFromCaster = false;
        public bool applyToCaster = false;
        public bool stacksScaleDuration;
        public bool stacksScalePower;

        public override void Apply(Prop ch, Character caster = null)
        {
            Prop sourceCh = applyFromCaster ? caster : ch;
            int numStacks = sourceCh.GetStacks(source);

            if (consume)
                sourceCh.RemoveStatus(source);

            Prop target = applyToCaster ? caster : ch;
            if (numStacks > 0 && target)
            {
                if (stacksScaleDuration)
                    duration *= numStacks;
                float power = stacksScalePower ? numStacks : 1.0f;
                target.ApplyStatus(resultStatus, duration, caster, null, power);
            }

        }

        public override string GetDescription(bool brief = false)
        {
            return (consume ? "Converts " : "Consumes ") + source.name + " to " + resultStatus.name;
        }

        public override bool isImmediate() { return true; }

        // used for AI evaluation
        public override float DamagePerHit()
        {
            return 0;
        }
    }
}