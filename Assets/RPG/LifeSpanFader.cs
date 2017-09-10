using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class LifeSpanFader : MonoBehaviour
    {
        ParticleSystem particles;
        Renderer rend;
        [HideInInspector]
        public Color color;
        [HideInInspector]
        public bool autoDestroy;
        Glow glow;

        float timer;
        [HideInInspector]
        public float lifespan = 1;

        // Use this for initialization
        void Start()
        {
            particles = GetComponent<ParticleSystem>();
            rend = GetComponent<Renderer>();
            glow = GetComponent<Glow>();

            timer = lifespan;
        }

        // used for persistent particles to restart them
        public void Restart()
        {
            gameObject.SetActive(true);
            if (particles)
                particles.Play();
            if (rend)
                rend.material.SetColor("_TintColor", color);
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
                    rend.material.SetColor("_TintColor", col);
                if (glow)
                    glow.UpdateGlow(timer);
            }
            if (timer < 0)
            {
                if (glow)
                    glow.Restore();
                if (autoDestroy)
                    Destroy(gameObject);
                else
                    gameObject.SetActive(false);
            }
        }
    }
}
