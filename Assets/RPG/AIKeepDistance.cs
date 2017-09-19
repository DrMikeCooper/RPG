using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AIKeepDistance", menuName = "RPG/AIKeepDistance", order = 4)]
    public class AIKeepDistance : AIAction
    {
        public float distance = 5;
        public float strength = 1;

        public override AIAction Execute(AIBrain brain)
        {
            if (brain.character.target)
            {
                Vector3 dir = brain.character.target.transform.position - brain.character.transform.position;
                dir.y = 0;
                dir.Normalize();
                Vector3 targetPoint = brain.target.transform.position + dir * distance;
                brain.MoveTo(targetPoint);
            }
            return this;
        }

        public override float Evaluate(AIBrain brain)
        {
            if (brain.character.target)
            {
                return Mathf.Abs(distance - Vector3.Distance(brain.character.target.transform.position, brain.character.transform.position)) * strength;
            }
            return 0;
        }

        public override float GetDuration()
        {
            return 0.5f;
        }
    }
}
