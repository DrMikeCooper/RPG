﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamRenderer : MonoBehaviour {

    LineRenderer lineRenderer;
    Transform source;
    Transform target;
    float timer;
    Vector3[] pts = new Vector3[2];
    
	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
	}

    public void Activate(Transform src, Transform tgt, float duration, Material mat, float beamWidth, Color col)
    {
        lineRenderer.enabled = true;
        timer = duration;
        source = src;
        target = tgt;
        lineRenderer.material = mat;
        lineRenderer.startColor = lineRenderer.endColor = col;
        lineRenderer.startWidth = lineRenderer.endWidth = beamWidth;
    }

	// Update is called once per frame
	void Update () {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                lineRenderer.enabled = false;

            // fade out at last 0.2 secs
            Color col = lineRenderer.startColor;
            col.a = timer * 5.0f;
            lineRenderer.startColor = lineRenderer.endColor = col;

            pts[0] = source.position;
            pts[1] = target.position;
            lineRenderer.SetPositions(pts);
        }
	}
}