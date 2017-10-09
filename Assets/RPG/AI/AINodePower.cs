using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AINodePower", menuName = "RPG/AI/AINodePower", order = 1)]
    public class AINodePower : AINode
    {
        // use this to add a condition to a power, otherwise use the power directly
        public Power power;
        public AICondition targetCondition;

        bool isTrue;

        public override float Evaluate(AIBrain brain)
        {
            // if our condition is false, don't do it
            if (!IsCondition(condition, brain.character))
            {
                brain.AddDebugMsg("-PW(not)" + power.name);
                return 0;
            }

            // we can use the power, so evaluate it
            return power.Evaluate(brain, targetCondition);
        }

        public override AIAction Execute(AIBrain brain)
        {
            brain.AddDebugMsg("+PW:" + power.name);
            return power.Execute(brain);
        }
    }
}
