using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class LifeSpanFader : MonoBehaviour
    {
        ParticleSystem particles;
        float timer;
        float lifespan = 1;

        // Use this for initialization
        void Start()
        {
            Destroy(gameObject, lifespan);
            particles = GetComponent<ParticleSystem>();
            timer = lifespan;
        }

        // Update is called once per frame
        void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 1)
            {
                Color col = particles.startColor;
                col.a = timer;
                particles.startColor = col;
            }
        }
    }
}
