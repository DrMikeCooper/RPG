using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    // destructable prop with health and a possible explosion
    public class Prop : MonoBehaviour
    {
        public float health;
        public PowerArea explosion;

        // Use this for initialization
        public void ApplyDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                if (explosion)
                    explosion.Explode(transform, null);
                Destroy(gameObject, 0.5f);
            }
        }
    }
}
