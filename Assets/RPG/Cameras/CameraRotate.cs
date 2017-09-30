using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class CameraRotate : MonoBehaviour
    {
        public Transform target;
        public float speed = 10;
        public float distance = 5;
        public float distanceMin = 2;
        public float distanceMax = 20;
        public float heightOffset = 1.5f;
        public float zoomSpeed = 100;

        float lastMouseX;
        float lastMouseY;

        // Update is called once per frame
        void Update()
        {
            distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, distanceMin, distanceMax);
            if (Input.GetMouseButton(1))
            {
                float deltaX = Input.mousePosition.x - lastMouseX;
                float deltaY = Input.mousePosition.y - lastMouseY;

                Vector3 angles = transform.eulerAngles + (Vector3.right * deltaY + Vector3.up * deltaX) * Time.deltaTime * speed;
                if (angles.x > 180)
                    angles.x -= 360;
                angles.x = Mathf.Clamp(angles.x, -70, 70);
                transform.eulerAngles = angles;
            }
            transform.position = target.position + Vector3.up * heightOffset - transform.forward * distance;

            lastMouseX = Input.mousePosition.x;
            lastMouseY = Input.mousePosition.y;
        }
    }
}