using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AINodeBranch", menuName = "RPG/AINodeBranch", order = 4)]
    public class AINodeBranch : AINode
    {
        public AINode nodeTrue;
        public AINode nodeFalse;

        public override float Evaluate(AIBrain brain)
        {
            return 0;
        }

        public override AINode Execute(AIBrain brain)
        {
            AINode node = IsCondition(condition, brain.character) ? nodeTrue : nodeFalse;
            return node.Execute(brain);
        }
    }
}
