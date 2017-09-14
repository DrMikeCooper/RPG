using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class NumberFloater : MonoBehaviour
    {
        float timer;
        TextMesh textMesh;

        static NumberFloater[] floaters;

        void Start()
        {
            textMesh = GetComponent<TextMesh>();
        }

        public void Activate(Transform t, string msg, Color color)
        {
            if (textMesh == null)
                textMesh = GetComponent<TextMesh>();

            gameObject.SetActive(true);
            timer = 0;
            transform.position = t.position;
            textMesh.text = "" + msg;
            textMesh.color = color;
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            transform.position += Vector3.up * Time.deltaTime * 3;
            float alpha = 3.0f - timer;
            if (alpha < 1)
            {
                Color col = textMesh.color;
                col.a = alpha;
                textMesh.color = col;
                if (alpha <= 0)
                    gameObject.SetActive(false);
            }

            // billboard
            transform.forward = Camera.main.transform.forward;
        }
    }
}
