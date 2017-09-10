using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    // contains a list of behaviours. 
    // it evaluates them all and picks the most suitable one each time

    [CreateAssetMenu(fileName = "AINodeEvaluate", menuName = "RPG/AINodeEvaluate", order = 4)]
    public class AINodeEvaluate : AINode
    {
        public AIAction[] behaviours;

        // TODO - member variables can be expected to last between Evaluate and Execute in the same function call, but feels fragile!
        AIAction bestBehaviour;
        float bestEvaluation = 0;

        public override float Evaluate(AIBrain brain)
        {
            bestBehaviour = null;
            for (int i = 0; i < behaviours.Length; i++)
            {
                float evaluation = behaviours[i].Evaluate(brain);
                if (bestBehaviour == null || evaluation > bestEvaluation)
                {
                    bestBehaviour = behaviours[i];
                    bestEvaluation = evaluation;
                }
            }

            return bestEvaluation;
        }

        public override AIAction Execute(AIBrain brain)
        {
            Evaluate(brain);
            return bestBehaviour;
        }

        public override float GetDuration()
        {
            return bestBehaviour.GetDuration();
        }
    }
}
