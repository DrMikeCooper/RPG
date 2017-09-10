using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AINodePower", menuName = "RPG/AINodePower", order = 4)]
    public class AINodePower : AINode
    {
        // use this to add a condition to a power, otherwise use the power directly
        public Power power;
        public AICondition targetCondition;

        public override float Evaluate(AIBrain brain)
        {
            return power.Evaluate(brain, targetCondition);
        }

        public override AIAction Execute(AIBrain brain)
        {
            return power.Execute(brain);
        }
    }
}
