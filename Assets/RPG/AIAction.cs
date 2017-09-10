using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public abstract class AIAction : ScriptableObject
    {
        public abstract AIAction Execute(AIBrain brain);
        public abstract float Evaluate(AIBrain brain);
        public abstract float GetDuration();
    }
}
