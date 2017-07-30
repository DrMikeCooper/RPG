using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class LifeSpan : MonoBehaviour
    {

        public float lifespan;

        // Use this for initialization
        void Start()
        {
            Destroy(gameObject, lifespan);
        }
    }
}
