using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamRenderer : MonoBehaviour {

    LineRenderer lineRenderer;
    Transform source;
    GameObject target;
    float timer;
    Vector3[] pts = new Vector3[2];
    public float uvSpeed = 1.0f;

    // Use this for initialization
    void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
	}

    public void Activate(Transform src, Transform tgt, float duration, Material mat, float beamWidth, Color col)
    {
        lineRenderer.enabled = true;
        timer = duration;
        source = src;
        target = tgt.gameObject;
        lineRenderer.material = mat;
        
        lineRenderer.startColor = lineRenderer.endColor = col;
        lineRenderer.startWidth = lineRenderer.endWidth = beamWidth;

        // set UV titling to match distance
        float distance = Vector3.Distance(src.position, tgt.position);
        lineRenderer.material.SetTextureScale("_MainTex", new Vector2(2*distance, 1));
    }

	// Update is called once per frame
	void Update ()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                lineRenderer.enabled = false;

            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(timer*uvSpeed, 0));

            // fade out at last 0.2 secs
            Color col = lineRenderer.startColor;
            col.a = timer * 5.0f;
            lineRenderer.startColor = lineRenderer.endColor = col;
            if (target)
            { 
                pts[0] = source.position;
                pts[1] = target.transform.position;
                lineRenderer.SetPositions(pts);
            }
        }
	}
}
