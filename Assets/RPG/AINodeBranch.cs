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

        bool decided = false;

        bool isTrue = false;
        public override float Evaluate(AIBrain brain)
        {
            decided = true;
            isTrue = IsCondition(condition, brain.character);

            AIAction node = isTrue ? nodeTrue : nodeFalse;
            brain.AddDebugMsg("-BR(" + isTrue + ")" + node.name);

            return node.Evaluate(brain);
        }

        public override AIAction Execute(AIBrain brain)
        {
            if (!decided)
                isTrue = IsCondition(condition, brain.character);

            AIAction node = isTrue ? nodeTrue : nodeFalse;
            brain.AddDebugMsg("+BR:" + isTrue + " "  + node.name);

            decided = false;

            return node.Execute(brain);
        }
    }
}
