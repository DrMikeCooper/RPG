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

        bool isTrue;

        public override float Evaluate(AIBrain brain)
        {
            // if our condition is false, don't do it
            if (!IsCondition(condition, brain.character))
                return 0;

            // return zero if the power's on cooldown or we don't have enough energy
            if (brain.character.GetCoolDown(power) > 0 || power.energyCost > brain.character.energy)
                return 0;

            // we can use the power, so evaluate it
            return power.Evaluate(brain, targetCondition);
        }

        public override AIAction Execute(AIBrain brain)
        {

            return power.Execute(brain);
        }
    }
}
