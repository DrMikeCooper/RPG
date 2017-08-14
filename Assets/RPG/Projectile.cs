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
        float charge;

        // Use this for initialization
        public void Init(Power p, Character c, Vector3 v)
        {
            startPos = c.transform.position;
            parentPower = p;
            caster = c;
            velocity = v;
            charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
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
            Prop prop = col.gameObject.GetComponent<Prop>();
            if (prop)
            {
                //prop.ApplyDamage(6);
            }

            Character ch = col.gameObject.GetComponent<Character>();
            if (ch != caster)
            {
                if (ch)
                {
                    // bullshit correction because the coordinates are coming out wierd here
                    Vector3 pos  = ch.transform.position;
                    pos.y = 0;
                    ch.transform.position = pos;

                    parentPower.Apply(ch, charge);
                }
                Destroy(gameObject);
            }
        }
    }
}