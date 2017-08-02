using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class Projectile : MonoBehaviour
    {
        Power parentPower;
        Character caster;
        Vector3 velocity;
        Vector3 startPos;

        float range;

        // Use this for initialization
        public void Init(Power p, Character c, Vector3 v)
        {
            startPos = c.transform.position;
            parentPower = p;
            caster = c;
            velocity = v;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += velocity * Time.deltaTime;
            if (Vector3.Distance(startPos, transform.position) > parentPower.range)
                Destroy(gameObject);
        }

        void OnTriggerEnter(Collider col)
        {
            Character ch = col.gameObject.GetComponent<Character>();
            if (ch != caster)
            {
                if (ch)
                    parentPower.Apply(ch);
                Destroy(gameObject);
            }
        }
    }
}