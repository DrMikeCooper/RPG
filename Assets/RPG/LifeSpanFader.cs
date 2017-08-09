using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class LifeSpanFader : MonoBehaviour
    {
        ParticleSystem particles;
        Renderer rend;
        Color color;
        [HideInInspector]
        public bool autoDestroy;

        float timer;
        [HideInInspector]
        public float lifespan = 1;

        // Use this for initialization
        void Start()
        {
            particles = GetComponent<ParticleSystem>();
            rend = GetComponent<TrailRenderer>();
            timer = lifespan;
        }

        // used for persistent particles to restart them
        public void Restart()
        {
            particles.Play();
            if (rend)
                rend.material.color = color;
            enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            timer -= Time.deltaTime;
            // fade out over the last second
            if (timer < 1)
            {
                if (particles)
                    particles.Stop();
                Color col = color;
                col.a = timer;
                if (rend)
                    rend.material.color = col;
            }
            if (timer < 0 && autoDestroy)
                Destroy(gameObject);
        }
    }
}
