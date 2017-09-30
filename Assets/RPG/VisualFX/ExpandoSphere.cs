using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandoSphere : MonoBehaviour {

    public float startRadius = 0.1f;
    public float endRadius = 1;
    public float lifeTime = 1;
    float timer;
    Renderer rend;
    Color color;

	// Use this for initialization
	void Start () {
        timer = 0;
        transform.localScale = Vector3.one * startRadius;
        rend = GetComponent<Renderer>();
        color = rend.material.GetColor("_TintColor");
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        float alpha = timer / lifeTime;
        if (alpha < 1)
        {
            transform.localScale = Vector3.one * (startRadius * (1 - alpha) + endRadius * alpha);
            if (alpha > 0.5)
            {
                Color col = color;
                col.a = 1.0f - 2.0f * alpha;
                if (rend)
                    rend.material.SetColor("_TintColor", col);
            }
        }
        else
            gameObject.SetActive(false);
    }
}
