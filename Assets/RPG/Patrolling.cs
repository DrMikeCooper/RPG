using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    // sits on a character gameobject - stores which points you're using and your character's data
    public class Patrolling : MonoBehaviour
    {
        public PatrolPoints path;
        public int currentNode;
        public bool forwards;

        void Start()
        {
            currentNode = -1;
        }

        public void Patrol(AIBrain brain)
        {
            if (currentNode == -1)
            {
                currentNode = Random.Range(0, path.points.Length);
                brain.MoveTo(path.points[currentNode]);
                return;
            }

            Vector3 target = path.points[currentNode].position;
            float dist = Vector3.Distance(gameObject.transform.position, target);
            if (dist < path.radius)
            {
                if (Random.Range(0, 100) < path.reverseChance)
                    forwards = !forwards;

                if (forwards)
                    currentNode++;
                else
                    currentNode--;
                currentNode = currentNode % path.points.Length;

                brain.MoveTo(path.points[currentNode], true);
            }
        }
    }
}
