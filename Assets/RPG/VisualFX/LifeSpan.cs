using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class LifeSpan : MonoBehaviour
    {
        public float lifespan;
        public float timer = 0;
        // Use this for initialization
        void Start()
        {
            timer = 0;
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > 0)
                ObjectFactory.Recycle(gameObject);
        }
    }
}
