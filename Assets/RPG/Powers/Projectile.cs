using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class Projectile : MonoBehaviour
    {
        Power parentPower;
        bool doStatus = true;
        Prop caster;
        Vector3 velocity;
        Vector3 startPos;

        float range;
        float charge;
        int chains;

        // Use this for initialization
        public void Init(Power p, Prop c, Vector3 v, bool ds)
        {
            startPos = c.transform.position;
            parentPower = p;
            doStatus = true;
            caster = c;
            velocity = v;
            charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
            chains = 0;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += velocity * Time.deltaTime;
            if (Vector3.Distance(startPos, transform.position) > parentPower.range)
                ObjectFactory.Recycle(gameObject);
            transform.forward = velocity;
        }

        void OnTriggerEnter(Collider col)
        {
            HitResponse.ReflectionType deflect = HitResponse.ReflectionType.None;
            bool finished = true;

            Prop ch = col.gameObject.GetComponent<Prop>();
            if (ch != caster)
            {
                if (ch)
                {
                    // check to see if we deflect it first
                    deflect = ch.GetReflection(parentPower.type);

                    bool apply = (deflect == HitResponse.ReflectionType.None);
                    bool bounce = !apply;
                    bool chaining = false;

                    if (chains < (parentPower as PowerProjectile).maxChains)
                    {
                        chains++;
                        bounce = true;
                        chaining = true;
                        deflect = HitResponse.ReflectionType.Redirect;
                    }

                    if (apply)
                    {
                        // apply the power normally

                        // bullshit correction because the coordinates are coming out weird here
                        Vector3 pos = ch.transform.position;
                        pos.y = 0;
                        ch.transform.position = pos;

                        // if we miss (based on accuracy), set the flag here so we dont destroy the projectile
                        if (!parentPower.Apply(ch, charge, caster as Character, doStatus))
                            finished = false;
                    }

                    if (bounce)
                    {
                        finished = false;
                        // reverse direction for now
                        int team = -1;
                        Character cha = (chaining ? caster : ch) as Character;
                        if (cha) team = cha.team;
                        float speed = velocity.magnitude;
                        velocity = speed * HitResponse.Reflect(ch, caster.transform.position, deflect, team);

                        startPos = transform.position;
                        if (!chaining)
                            caster = ch as Character;
                    }
                }
                if (finished)
                    ObjectFactory.Recycle(gameObject);
            }
        }
    }
}