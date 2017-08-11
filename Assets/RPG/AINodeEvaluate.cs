using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AINodeEvaluate", menuName = "RPG/AINodeEvaluate", order = 4)]
    public class AINodeEvaluate : AINode
    {
        public override float Evaluate(AIBrain brain)
        {
            return 0;
        }

        public override AINode Execute(AIBrain brain)
        {
            // evaluate all child nodes and pick the best one?
            return null;
        }
    }
}
