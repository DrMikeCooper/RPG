using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AINodePower", menuName = "RPG/AINodePower", order = 4)]
    public class AINodePower : AINode
    {
        public Power power;

        public AICondition targetCondition;
        public override float Evaluate(AIBrain brain)
        {
            return power.Evaluate(brain, targetCondition);
        }

        public override AINode Execute(AIBrain brain)
        {
            power.Evaluate(brain, targetCondition);
            brain.target = power.npcTarget;

            power.UpdateAction(brain);

            return this;
        }
    }
}
