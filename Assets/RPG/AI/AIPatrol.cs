using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class AIPatrol : AIAction
    {
        
        public override AIAction Execute(AIBrain brain)
        {
            brain.AddDebugMsg("+PATROL");
            brain.patrolling.Patrol(brain);
            return this;
        }

        public override float Evaluate(AIBrain brain)
        {
            // always low - the aim of patrol is to move the character around until they see a target
            return 0.05f;
        }

        public override float GetDuration()
        {
            return 0.5f;
        }
    }
}
