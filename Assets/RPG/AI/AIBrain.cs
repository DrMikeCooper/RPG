using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class AIBrain : MonoBehaviour, IMenuItemResponder
    {
        public AIAction[] behaviours;

        [HideInInspector]
        public AIAction rootNode;

        // sibling components
        [HideInInspector]
        public Character character;
        [HideInInspector]
        AICharacterControl ai;

        // dynamic data
        [HideInInspector]
        public float countDown;
        [HideInInspector]
        public Character target; // gets set by the Evaluate routines
        public float closingRange; // true if we're currently moving about

        public List<Character> enemies = new List<Character>(); // array of enemies that we are aware of
        public List<Character> allies = new List<Character>();

        [HideInInspector]
        public Patrolling patrolling;

        public bool showDebug = false;
        public Text debugText;
        public Text debugCountdown;

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            ai = GetComponent<AICharacterControl>();

            List<AIAction> extras = new List<AIAction>();
            Patrolling patrol = GetComponent<Patrolling>();
            if (patrol)
                extras.Add(ScriptableObject.CreateInstance<AIPatrol>());

            int numActions = behaviours.Length + extras.Count;

            // set up a suitable rootnode, either a single power, or a group of them which compete!
            if (numActions == 0)
            {
                rootNode = null;
            }
            else if (numActions == 1)
            {
                if (behaviours.Length > 0)
                    rootNode = behaviours[0].MakeInstance();
                else
                    rootNode = extras[0].MakeInstance();
            }
            else
            {
                AIAction[] actions = new AIAction[numActions];
                for (int i = 0; i < behaviours.Length; i++)
                    actions[i] = behaviours[i].MakeInstance();
                for (int i = 0; i < extras.Count; i++)
                    actions[i + behaviours.Length] = extras[i].MakeInstance();

                AINodeEvaluate evalNode = ScriptableObject.CreateInstance<AINodeEvaluate>();
                evalNode.behaviours = actions;
                rootNode = evalNode;
            }
            patrolling = GetComponent<Patrolling>();
        }

        public void SetRootNode(AIAction action)
        {
            rootNode = action;
            countDown = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (character.dead)
            {
                ai.SetTarget(null);
                return;
            }

            if (character.isHeld())
                ai.SetTarget(null);

            if (countDown > 0)
            {
                countDown -= Time.deltaTime;
                if (character.isHeld() == false && character.target)
                    character.FaceTarget();
                if (debugCountdown)
                    debugCountdown.text = "" + countDown;
            }
            else
            {
                if (character.isHeld())
                {
                    countDown = 1;
                }
                else
                {
                    UpdateEnemies();

                    if (showDebug)
                    {
                        //Debug.Log(RPGSettings.instance.GetHUD(character).debugText.text);
                        RPGSettings.instance.GetHUD(character).ClearDebugText();
                        foreach (Character enemy in enemies)
                            RPGSettings.instance.GetHUD(enemy).ClearDebugText();
                        foreach (Character ally in allies)
                            RPGSettings.instance.GetHUD(ally).ClearDebugText();
                    }
                    closingRange = 0;

                    //Debug.Log("THINKING...");
                    Power p = rootNode as Power;
                    if (p)
                        p.npcTarget = null;

                    AIAction node = rootNode ? rootNode.Execute(this): null;
                    //Debug.Log("...THINKING");
                    countDown = node == null ? 3 : node.GetDuration();
                }
            }

            if (character.activePower)
                character.activePower.OnUpdate(character);

            if (closingRange > 0 && Power.WithinRange(character, closingRange, true)) 
                countDown = 0;
        }

        public void MoveTo(Transform pos, bool walking = false)
        {
            GetComponent<ThirdPersonCharacter>().walking = walking;
            ai.target = pos;
        }

        public void MoveTo(Vector3 pos, bool walking = false)
        {
            GetComponent<ThirdPersonCharacter>().walking = walking;
            ai.SetTarget(null);
            ai.SetTargetPos(pos);
        }

        public void ResetEnemies()
        {
            enemies.Clear();
            allies.Clear();
        }

        public void UpdateEnemies()
        {
            // check new enemies for line of sight and add to our list
            foreach (Character ch in Power.getAll())
            {
                if (ch.team != character.GetTeam() && enemies.Contains(ch) == false)
                {
                    if (character.CanSee(ch))
                    {
                        enemies.Add(ch);
                    }
                }

                if (ch.team == character.GetTeam() && allies.Contains(ch) == false)
                {
                    if (character.CanSee(ch))
                    {
                        allies.Add(ch);
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

            deathRow.Clear();
            foreach (Character ch in allies)
                if (ch.dead || ch.gameObject.activeSelf == false)
                    deathRow.Add(ch);

            foreach (Character ch in deathRow)
                allies.Remove(ch);

        }

        public void MakeAwareOf(Character ch)
        {
            if (ch && ch.team != character.GetTeam() && enemies.Contains(ch) == false)
                enemies.Add(ch);
        }

        public void AddDebugMsg(string msg)
        {
            if (showDebug)
                RPGSettings.instance.GetHUD(character).AddDebugText(msg);
        }

        public void OnButtonDown(MenuItem item)
        {
        }

        public void OnButtonUp(MenuItem item)
        {
            SetRootNode(item.action.MakeInstance()); // TODO  - make sure Character has a copy
            Power power = rootNode as Power;
            if (power)
                power.npcTarget = target as Character;
        }

    }
}
