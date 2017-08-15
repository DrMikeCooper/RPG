using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class TabTarget : MonoBehaviour
    {
        public GameObject reticle;
        Character user;

        // Use this for initialization
        void Start()
        {
            reticle.SetActive(false);
            user = GetComponent<Character>();
        }

        // Update is called once per frame
        void Update()
        {
            if (user.target)
            {
                reticle.transform.position = user.target.transform.position;
            }

            // no changing targets while a power is active
            if (user.activePower)
                return;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                List<Character> targets = new List<Character>();

                foreach (Character ch in PowerArea.getAll())
                {
                    // left shift-TAB toggles between allies, normla TAB through enemies
                    bool valid = Input.GetKey(KeyCode.LeftShift) ?
                        (ch != user && ch.team == user.team)
                        : (ch.team != user.team);
                    if (valid)
                    {
                        Vector3 clipPos = Camera.main.WorldToViewportPoint(ch.transform.position);
                        if (clipPos.z > 0 && clipPos.x >= 0 && clipPos.x <= 1 && clipPos.y >= 0 && clipPos.y <= 1)
                        {
                            targets.Add(ch);
                            ch.xScreen = clipPos.x;
                        }
                    }
                }

                // sort from left to right
                targets.Sort(delegate (Character a, Character b) {
                    return a.xScreen.CompareTo(b.xScreen);
                });

                int selected = -1;
                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i] == user.target)
                        selected = i;
                }

                //move to the next one with wraparound
                selected++;
                if (selected >= targets.Count)
                    selected = 0;

                user.target = targets[selected];
                reticle.SetActive(true);
            }

            // left mouse clicks select a character (or none)
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit info;
                if (Physics.Raycast(ray, out info))
                {
                    user.target = info.collider.GetComponent<Prop>();

                    //GameObject go = GameObject.Find(user.target.name);
                    //Prop p = go ? go.GetComponent<Prop>() : null;

                    reticle.SetActive(user.target != null);
                }
            }
        }

        public Prop GetTarget()
        {
            return user.target;
        }
    }
}
