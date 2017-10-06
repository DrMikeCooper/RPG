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
            alpha = 1 - (1 - alpha) * (1 - alpha);
            transform.localScale = Vector3.one * (startRadius * (1 - alpha) + endRadius * alpha);
        }
        else if (alpha < 1.5f)
        {
            alpha = (1.5f - alpha) * 2;
            Color col = color;
            col.a = alpha;
            if (rend)
                rend.material.SetColor("_TintColor", col);
        }
        else
            gameObject.SetActive(false);
    }
}
