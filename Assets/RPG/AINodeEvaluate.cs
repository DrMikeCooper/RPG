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
        float[] evals;
        Prop[] targets;

        // TODO - member variables can be expected to last between Evaluate and Execute in the same function call, but feels fragile!
        AIAction bestBehaviour;
        float bestEvaluation = 0;

        public override float Evaluate(AIBrain brain)
        {
            // if the caster condition fails, don't do it!
            if (!IsCondition(condition, brain.character)) return 0;

            bestBehaviour = null;
            bestEvaluation = 0;
            evals = new float[behaviours.Length];
            targets = new Prop[behaviours.Length];
            for (int i = 0; i < behaviours.Length; i++)
            {
                float evaluation = behaviours[i].Evaluate(brain);
                evals[i] = evaluation;
                Power p = behaviours[i] as Power;
                if (p == null)
                {
                    AINodePower pn = behaviours[i] as AINodePower;
                    if (pn) p = pn.power;
                }
                if (p)
                    targets[i] = p.npcTarget;
                if (evaluation > bestEvaluation)
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

            string msg = "+EV:" + (bestBehaviour ? bestBehaviour.name : "null") + "(" + bestEvaluation + ")";
            brain.AddDebugMsg(msg);

            if (bestBehaviour)
                bestBehaviour.Execute(brain);
            return bestBehaviour;
        }

        public override float GetDuration()
        {
            return bestBehaviour == null ? 3 : bestBehaviour.GetDuration();
        }

        public override AIAction MakeInstance()
        {
            AINodeEvaluate eval = base.MakeInstance() as AINodeEvaluate;
            for (int i = 0; i < behaviours.Length; i++)
                eval.behaviours[i] = eval.behaviours[i].MakeInstance();
            return eval;
        }
    }
}
