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
            Status,
        };

        [System.Serializable]
        public struct AICondition
        {
            public AICondition(AIConditionType t) { type = t; threshold = 0; reverse = false; status = null; }
            public AIConditionType type;
            public float threshold;
            public bool reverse;
            public Status status;
        }

        public static AICondition always = new AICondition(AIConditionType.Always);

        public float duration;

        // the condition to check against the caster before using
        public AICondition condition;

        // functions to override
        public override float GetDuration() { return duration; }

        public static bool IsCondition(AICondition condition, Prop prop, Power p = null)
        {
            bool isTrue = true;
            Character ch = prop as Character;
            switch (condition.type)
            {
                case AIConditionType.Held: isTrue = ch == null || ch.isHeld(); break;
                case AIConditionType.Rooted: isTrue = ch == null || ch.isRooted(); break;
                case AIConditionType.Health: isTrue = prop.GetHealthPct() < condition.threshold; break;
                case AIConditionType.Energy: isTrue = ch == null || ch.GetEnergyPct() < condition.threshold; break;
                case AIConditionType.Random: isTrue = Random.Range(0, 100) < condition.threshold; break;
                case AIConditionType.Shielded: isTrue =  p && prop.stats[RPGSettings.GetResistanceStat(p.type)].currentValue > condition.threshold; break;
                case AIConditionType.Boosted: isTrue = p && ch && ch.stats[RPGSettings.GetDamageStat(p.type)].currentValue > condition.threshold; break;
                case AIConditionType.Status: isTrue = prop.GetStacks(condition.status) > 0; break;
            }
            if (condition.reverse)
                isTrue = !isTrue;
            return isTrue;
        }
    }
}
