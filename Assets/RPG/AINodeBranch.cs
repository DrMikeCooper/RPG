using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AINodeBranch", menuName = "RPG/AINodeBranch", order = 4)]
    public class AINodeBranch : AINode
    {
        public AIAction nodeTrue;
        public AIAction nodeFalse;

        bool isTrue = false;
        public override float Evaluate(AIBrain brain)
        {
            isTrue = IsCondition(condition, brain.character);
            if (isTrue)
                return nodeTrue.Evaluate(brain);
            else
                return nodeFalse.Evaluate(brain);
        }

        public override AIAction Execute(AIBrain brain)
        {
            AIAction node = isTrue ? nodeTrue : nodeFalse;
            return node.Execute(brain);
        }
    }
}
