using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for
        public Vector3? targetPos = null;

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }


        private void Update()
        {
            if (target != null && character.maxSpeed > 0)
                agent.SetDestination(target.position);
            else if (targetPos != null)
            {
                agent.SetDestination(targetPos.Value);
                if (Vector3.Distance(transform.position, targetPos.Value) < 1.0f)
                {
                    agent.SetDestination(transform.position);
                    targetPos = null;
                }
            }

            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);
            else
                character.Move(Vector3.zero, false, false);
        }

        public void SetTargetPos(Vector3 pos)
        {
            targetPos = pos;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            if (target == null)
                agent.SetDestination(transform.position);
        }
    }
}
