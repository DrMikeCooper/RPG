using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class NPCPowers : MonoBehaviour
    {
        Power currentPower;
        [HideInInspector]
        public Character character;
        public float timer; // gets set by the Evaluate routines
        public Character target; // gets set by the Evaluate routines
        UnityStandardAssets.Characters.ThirdPerson.AICharacterControl ai;

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            ai = GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>();
        }

        // Update is called once per frame
        void Update()
        {
            if (character.activePower)
                character.activePower.OnUpdate(character);

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                return;
            }

            // find the best action each frame (TODO - not every frame?)
            Power best = GetBestAction();
            target = best ? best.npcTarget : null;

            // if it’s different from what we were doing, switch the FSM
            if (best != currentPower)
            {
                if (currentPower)
                    currentPower.Exit(this);
                currentPower = best;
                if (currentPower)
                    currentPower.Enter(this);
            }

            // update the current action
            if (currentPower)
                currentPower.UpdateAction(this);
        }

        // checks all our available actions and evaluates each one, getting the best
        Power GetBestAction()
        {
            if (character.isHeld())
                return null;

            Power action = null;
            float bestValue = 0;

            foreach (Power a in character.powers)
            {
                float value = a.Evaluate(this);
                if (action == null || value > bestValue)
                {
                    action = a;
                    bestValue = value;
                }
            }

            return action;
        }

        public void MoveTo(Transform pos)
        {
            ai.target = pos;
        }
    }
}