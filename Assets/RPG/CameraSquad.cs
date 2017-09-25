using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class CameraSquad : MonoBehaviour
    {
        // character we're tracking
        public Transform target;
       
        // height above the object
        public Vector3 offset = new Vector3(1,9,1);

        public float border = 64;
        float speed = 40.0f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // track the currently selected squad member of we have one
            if (target)
                transform.position = target.position + offset;
            transform.forward = -offset;

            // if we go to the edge of the screen, scroll in that direction
            Vector3 mp = Input.mousePosition;
            float dx = 0, dy = 0;
            if (mp.x < border) dx = -1;
            if (mp.x > Screen.width - border) dx = 1;
            if (mp.y < border) dy = -1;
            if (mp.y > Screen.height - border) dy = 1;

            if (dx != 0 || dy != 0)
            {
                target = null;
                // maintain our height
                float y = transform.position.y;
                Vector3 pos = transform.position + speed * Time. deltaTime * (transform.right * dx + transform.up * dy);
                pos.y = y;
                transform.position = pos;
            }
        }
    }
}
