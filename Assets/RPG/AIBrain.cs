using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class AIBrain : MonoBehaviour
    {
        public AINode rootNode;

        // sibling components
        [HideInInspector]
        public Character character;
        [HideInInspector]
        UnityStandardAssets.Characters.ThirdPerson.AICharacterControl ai;

        // dynamic data
        [HideInInspector]
        public float countDown;
        [HideInInspector]
        public Character target; // gets set by the Evaluate routines

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            ai = GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>();
        }

        // Update is called once per frame
        void Update()
        {
            if (countDown > 0)
            {
                countDown -= Time.deltaTime;
            }
            else
            {
                AINode node = rootNode.Execute(this);
                countDown = node.duration;
            }

            if (character.activePower)
                character.activePower.OnUpdate(character);
        }

        public void MoveTo(Transform pos)
        {
            ai.target = pos;
        }
    }
}
