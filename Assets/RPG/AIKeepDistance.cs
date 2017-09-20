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
        public enum FollowTarget
        {
            Target,
            Player
        }

        static GameObject player;
        public FollowTarget follow;

        static GameObject GetPlayer()
        {
              if (player == null)
                  player = GameObject.FindGameObjectWithTag("Player");
            return player;
        }

        public override AIAction Execute(AIBrain brain)
        {
            GameObject target = null;
            switch (follow)
            {
                case FollowTarget.Player:
                    target = GetPlayer();
                    break;
                case FollowTarget.Target:
                    target = brain.character.target.gameObject;
                    break;
            }
        
            if (target)
            {
                Vector3 dir =  brain.character.transform.position - target.transform.position;
                dir.y = 0;
                dir.Normalize();
                Vector3 targetPoint = target.transform.position + dir * distance;
                brain.MoveTo(targetPoint);
            }
            return this;
        }

        public override float Evaluate(AIBrain brain)
        {
            GameObject target = brain.character.target ? brain.character.target.gameObject : null;
            if (follow == FollowTarget.Player)
                target = GetPlayer();

            if (target == null)
                return 0;

            float dist = Mathf.Abs(distance - Vector3.Distance(target.transform.position, brain.transform.position));
            dist = Mathf.Max(dist - 1, 0);

            brain.character.AddAIDebugText(brain.character, "Follow " + dist * 0.1f);
            return dist * 0.1f;
        }

        public override float GetDuration()
        {
            return 0.5f;
        }
    }
}
