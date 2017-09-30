using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class NumberFloater : MonoBehaviour
    {
        float timer;
        TextMesh textMesh;
        Text text;
        static NumberFloater[] floaters;

        void Start()
        {
            textMesh = GetComponent<TextMesh>();
            text = GetComponent<Text>();
        }

        public void Activate(Transform t, string msg, Color color)
        {
            if (textMesh == null)
                textMesh = GetComponent<TextMesh>();
            if (text == null)
                text = GetComponent<Text>();

            gameObject.SetActive(true);
            timer = 0;
            transform.position = t.position;
            if (textMesh)
            {
                textMesh.text = "" + msg;
                textMesh.color = color;
            }
            if (text)
            {
                text.text = "" + msg;
                text.color = color;
            }
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            transform.position += Vector3.up * Time.deltaTime * 3;
            float alpha = 3.0f - timer;
            if (alpha < 1)
            {
                if (textMesh)
                {
                    Color col = textMesh.color;
                    col.a = alpha;
                    textMesh.color = col;
                }
                if (text)
                {
                    Color col = text.color;
                    col.a = alpha;
                    text.color = col;
                    GetComponent<RectTransform>().position = transform.position;
                }
                if (alpha <= 0)
                    gameObject.SetActive(false);
            }

            // billboard
            transform.forward = Camera.main.transform.forward;
        }
    }
}
