using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class SquadMember : MonoBehaviour
    {
        public int index;
        public bool selected = false;

        [HideInInspector]
        public Vector3 deltaPos; // used for group movement
        [HideInInspector]
        public float deltaAngle;

        public static Color targetColor = new Color(1,1,0.2f, 0.7f);

        [HideInInspector]
        public Character ch;

        AICharacterControl ai;

        // Use this for initialization
        void Start()
        {
            ai = GetComponent<AICharacterControl>();
            ch = GetComponent<Character>();
        }

        // Update is called once per frame
        void Update()
        {
            if (selected)
                ch.ShowReticle(targetColor);
            else
                ch.GetReticle().SetActive(false);
        }

        public void MoveTo(Vector3 pos, bool walking = false)
        {
            //GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>().walking = walking;
            ai.SetTarget(null);
            ai.SetTargetPos(pos);
        }

        public void SetDeltaPos(Vector3 dp)
        {
            deltaPos = dp;
            deltaAngle = Mathf.Atan2(dp.z, dp.x);
        }
    }
}
