using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class Projectile : MonoBehaviour
    {
        Power parentPower;
        Prop caster;
        Vector3 velocity;
        Vector3 startPos;

        float range;
        float charge;

        // Use this for initialization
        public void Init(Power p, Prop c, Vector3 v)
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
            bool deflect = false;

            Prop ch = col.gameObject.GetComponent<Prop>();
            if (ch != caster)
            {
                if (ch)
                {
                    // check to see if we deflect it first
                    foreach (HitResponse hr in ch.hitResponses)
                    {
                        if (hr.reflection && Random.Range(0, 100) < hr.percentage && (hr.damageType&parentPower.type)!=0)
                            deflect = true;
                    }
                    if (!deflect)
                    {
                        // apply the power normally

                        // bullshit correction because the coordinates are coming out weird here
                        Vector3 pos = ch.transform.position;
                        pos.y = 0;
                        ch.transform.position = pos;

                        if (!parentPower.Apply(ch, charge, caster as Character))
                            deflect = true;
                    }
                    else
                    {
                        // reverse direction for now
                        velocity = -velocity;
                        startPos = transform.position;
                        caster = ch as Character;
                    }
                }
                if (!deflect)
                    Destroy(gameObject);
            }
        }
    }
}