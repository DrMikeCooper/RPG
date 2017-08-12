﻿using System.Collections;
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
        public float closingRange; // true if we're currently moving about

        float checkCounter = 0;
        public List<Character> enemies = new List<Character>(); // array of enemies that we are aware of
        const float checkFrequency = 1.0f;

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            ai = GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>();
        }

        // Update is called once per frame
        void Update()
        {
            checkCounter -= Time.deltaTime;
            if (checkCounter <= 0)
            {
                UpdateEnemies();
                checkCounter = checkFrequency;
            }

            if (countDown > 0)
            {
                countDown -= Time.deltaTime;
            }
            else
            {
                closingRange = 0;
                AINode node = rootNode.Execute(this);
                countDown = node.duration;
            }

            if (character.activePower)
                character.activePower.OnUpdate(character);

            if (closingRange > 0 && Power.WithinRange(character, closingRange, true)) 
                countDown = 0;
        }

        public void MoveTo(Transform pos)
        {
            ai.target = pos;
        }

        public void UpdateEnemies()
        {
            foreach (Character ch in Power.getAll())
            {
                if (ch.team != character.team && enemies.Contains(ch) == false)
                {
                    if (character.CanSee(ch))
                    {
                        enemies.Add(ch);
                    }
                }
            }
        }

        public void MakeAwareOf(Character ch)
        {
            if (ch.team != character.team && enemies.Contains(ch) == false)
                enemies.Add(ch);
        }
    }
}
