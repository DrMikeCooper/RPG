﻿using System.Collections;
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
            HitResponse.ReflectionType deflect = HitResponse.ReflectionType.None;

            Prop ch = col.gameObject.GetComponent<Prop>();
            if (ch != caster)
            {
                if (ch)
                {
                    // check to see if we deflect it first
                    foreach (HitResponse hr in ch.hitResponses)
                    {
                        if (hr.reflection != HitResponse.ReflectionType.None && Random.Range(0, 100) < hr.percentage && (hr.damageType&parentPower.type)!=0)
                            deflect = hr.reflection;
                    }
                    if (deflect == HitResponse.ReflectionType.None)
                    {
                        // apply the power normally

                        // bullshit correction because the coordinates are coming out weird here
                        Vector3 pos = ch.transform.position;
                        pos.y = 0;
                        ch.transform.position = pos;

                        // if we miss (based on accuracy), set the flag here so we dont destroy the projectile
                        if (!parentPower.Apply(ch, charge, caster as Character))
                            deflect = HitResponse.ReflectionType.Deflect;
                    }
                    else
                    {
                        // reverse direction for now
                        int team = -1;
                        Character cha = ch as Character;
                        if (cha) team = cha.team;
                        float speed = velocity.magnitude;
                        velocity = speed * HitResponse.Reflect(ch, caster.transform.position, deflect, team);

                        startPos = transform.position;
                        caster = ch as Character;
                    }
                }
                if (deflect == HitResponse.ReflectionType.None)
                    Destroy(gameObject);
            }
        }
    }
}