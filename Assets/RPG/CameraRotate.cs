using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class CameraRotate : MonoBehaviour
    {
        public float speed = 10;
        float lastMouseX;
        float lastMouseY;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                float deltaX = Input.mousePosition.x - lastMouseX;
                float deltaY = Input.mousePosition.y - lastMouseY;
                Camera.main.transform.eulerAngles +=(Vector3.right * deltaY + Vector3.up * deltaX) * Time.deltaTime * speed;
            }
            lastMouseX = Input.mousePosition.x;
            lastMouseY = Input.mousePosition.y;
        }
    }
}