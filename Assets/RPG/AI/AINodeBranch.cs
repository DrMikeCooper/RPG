using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AINodeBranch", menuName = "RPG/AI/AINodeBranch", order = 1)]
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

        public override AIAction MakeInstance()
        {
            AINodeBranch branch = base.MakeInstance() as AINodeBranch;
            branch.nodeTrue = nodeTrue.MakeInstance();
            branch.nodeFalse = nodeFalse.MakeInstance();
            return branch;
        }
    }
}
