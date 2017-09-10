﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class AIBrain : MonoBehaviour
    {
        public AIAction[] behaviours;

        [HideInInspector]
        public AIAction rootNode;

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

        public List<Character> enemies = new List<Character>(); // array of enemies that we are aware of

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            ai = GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>();

            // set up a suitable rootnode, either a single power, or a group of them which compete!
            if (behaviours.Length == 1)
                rootNode = behaviours[0];
            else
            {
                AINodeEvaluate evalNode = ScriptableObject.CreateInstance<AINodeEvaluate>();
                evalNode.behaviours = behaviours;
                rootNode = evalNode;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (character.dead)
                return;

            if (countDown > 0)
            {
                countDown -= Time.deltaTime;
            }
            else
            {
                UpdateEnemies();
                closingRange = 0;
                AIAction node = rootNode.Execute(this);
                countDown = node.GetDuration();
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
            // check new enemies for line of sight and add to our list
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

            // remove Characters who are no longer on the map
            List<Character> deathRow = new List<Character>();
            foreach (Character ch in enemies)
                if (ch.dead || ch.gameObject.activeSelf == false)
                    deathRow.Add(ch);

            foreach (Character ch in deathRow)
                enemies.Remove(ch);
        }

        public void MakeAwareOf(Character ch)
        {
            if (ch.team != character.team && enemies.Contains(ch) == false)
                enemies.Add(ch);
        }
    }
}
