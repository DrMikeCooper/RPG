using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public abstract class AINode : AIAction
    {
        public enum AICondition
        {
            Always,
            Held,
            Rooted,
            HalfHealth,
            QuarterHealth,
            Percent75,
            Percent50,
            Percent25,
            Vulnerable,
            Shielded,
        };
        public float duration;

        // the condition to check against the caster before using
        public AICondition condition = AICondition.Always;

        // functions to override
        public override float GetDuration() { return duration; }

        public static bool IsCondition(AICondition condition, Character ch, Power p = null)
        {
            switch (condition)
            {
                case AICondition.Held: return ch.isHeld();
                case AICondition.Rooted: return ch.isRooted();
                case AICondition.HalfHealth: return ch.health <= ch.maxHealth * 0.5f;
                case AICondition.QuarterHealth: return ch.health <= ch.maxHealth * 0.25f;
                case AICondition.Percent75: return Random.Range(0, 100) < 75;
                case AICondition.Percent50: return Random.Range(0, 100) < 50;
                case AICondition.Percent25: return Random.Range(0, 100) < 25;
                case AICondition.Vulnerable:
                    {
                        if (p)
                        {
                            return ch.stats[RPGSettings.GetResistanceStat(p.type)].currentValue < 0;
                        }
                        return false; 
                    }
                case AICondition.Shielded:
                    {
                        if (p)
                        {
                            return ch.stats[RPGSettings.GetResistanceStat(p.type)].currentValue > 0;
                        }
                        return false; 
                    }
            }
            return true;
        }
    }
}
