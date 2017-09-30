using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public abstract class AINode : AIAction
    {
        public enum AIConditionType
        {
            Always,
            Held,
            Rooted,
            Health,
            Energy,
            Random,
            Shielded,
            Boosted,
        };

        [System.Serializable]
        public struct AICondition
        {
            public AICondition(AIConditionType t) { type = t; threshold = 0; reverse = false; }
            public AIConditionType type;
            public float threshold;
            public bool reverse;
        }

        public static AICondition always = new AICondition(AIConditionType.Always);

        public float duration;

        // the condition to check against the caster before using
        public AICondition condition;

        // functions to override
        public override float GetDuration() { return duration; }

        public static bool IsCondition(AICondition condition, Character ch, Power p = null)
        {
            bool isTrue = true;
            switch (condition.type)
            {
                case AIConditionType.Held: isTrue = ch.isHeld(); break;
                case AIConditionType.Rooted: isTrue = ch.isRooted(); break;
                case AIConditionType.Health: isTrue = ch.GetHealthPct() < condition.threshold; break;
                case AIConditionType.Energy: isTrue = ch.GetEnergyPct() < condition.threshold; break;
                case AIConditionType.Random: isTrue = Random.Range(0, 100) < condition.threshold; break;
                case AIConditionType.Shielded: isTrue = ch.stats[RPGSettings.GetResistanceStat(p.type)].currentValue > condition.threshold; break;
                case AIConditionType.Boosted: isTrue = ch.stats[RPGSettings.GetDamageStat(p.type)].currentValue > condition.threshold; break;
            }
            if (condition.reverse)
                isTrue = !isTrue;
            return isTrue;
        }
    }
}
