using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class TabTarget : MonoBehaviour
    {
        Character[] targets;
        int index = -1;
        public GameObject reticle;
        Character user;

        // Use this for initialization
        void Start()
        {
            targets = GameObject.FindObjectsOfType<Character>();
            reticle.SetActive(false);
            user = GetComponent<Character>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                index++;
                if (index >= targets.Length)
                    index = 0;
                reticle.SetActive(true);
            }

            if (index > -1)
            {
                GameObject target = GetTarget().gameObject;
                reticle.transform.position = target.transform.position;
            }

            // left mouse clicks select a character
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit info;
                if (Physics.Raycast(ray, out info))
                {
                    Character ch = info.collider.GetComponent<Character>();
                    index = GetIndex(ch);
                    reticle.SetActive(index != -1);
                }
            }
            user.target = GetTarget();
        }

        public Character GetTarget()
        {
            if (index < 0)
                return null;
            return targets[index];
        }

        int GetIndex(Character ch)
        {
            for (int i = 0; i < targets.Length; i++)
                if (ch == targets[i])
                    return i;
            return -1;
        }
    }
}
