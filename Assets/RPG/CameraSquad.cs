using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        public EventSystem[] eventSystems;
        Vector3 lastMousePos;
        bool ignoreScrolling = false;

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

            // check if the mouse is over a menu item first
            Vector3 mousePos = Input.mousePosition;
            if (mousePos != lastMousePos)
                ignoreScrolling = false;
            foreach (EventSystem e in eventSystems)
                if (e.IsPointerOverGameObject())
                    ignoreScrolling = true;
            lastMousePos = mousePos;

            // if we go to the edge of the screen, scroll in that direction
            if (!ignoreScrolling)
            {
                float dx = 0, dy = 0;
                if (mousePos.x < border) dx = -1;
                if (mousePos.x > Screen.width - border) dx = 1;
                if (mousePos.y < border) dy = -1;
                if (mousePos.y > Screen.height - border) dy = 1;

                if (dx != 0 || dy != 0)
                {
                    target = null;
                    // maintain our height
                    float y = transform.position.y;
                    Vector3 pos = transform.position + speed * Time.deltaTime * (transform.right * dx + transform.up * dy);
                    pos.y = y;
                    transform.position = pos;
                }
            }
        }
    }
}
