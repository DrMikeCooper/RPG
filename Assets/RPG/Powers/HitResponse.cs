using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "HitResponse", menuName = "RPG/HitResponse", order = 2)]
    public class HitResponse : Status
    {
        [Tooltip("Type of damage this applies to")]
        public RPGSettings.DamageType damageType = RPGSettings.DamageType.All;

        [Tooltip("Chance of this response triggering when hit")]
        public int percentage;

        
        public enum ReflectionType
        {
            None,
            Reflect,
            Redirect,
            Deflect
        };

        [Tooltip("Reflect: straight back to firer, Redirect: to random enemy, Deflect : Random direction")]
        public ReflectionType reflection;

        [Tooltip("Damage multiplied by this number goes to the named stat")]
        public float absorbFactor = 0;
        [Tooltip("An effect that plays when you absorb damage")]
        public Status[] absorbEffects;

        [Tooltip("Visual FX to play on activation")]
        public VisualEffect responseFX;
        public Character.BodyPart responseBodyPart;

        public override void Apply(Prop ch, Character caster = null)
        {
            ch.hitResponses.Add(this);
        }

        public void OnHit(Prop prop, float damage)
        {
            if (UnityEngine.Random.Range(0, 100) > percentage)
                return;

            if (absorbFactor > 0)
            {
                PlayEffect(prop);
                foreach(Status absorbEffect in absorbEffects)
                    prop.ApplyStatus(absorbEffect, damage * absorbFactor, prop as Character, this);
            }
        }

        // utility function for chained projectiles/beams and deflection code
        public static Vector3 Reflect(Prop prop, Vector3 source, ReflectionType type, int team)
        {
            Vector3 position = prop.transform.position;
            Vector3 newDir;

            // for simple types, just reflect straight back
            if (type == ReflectionType.None || type == ReflectionType.Reflect)
            {
                return (source - position).normalized;
            }

            // for redirection, try finding a target to redirect at
            if (type == ReflectionType.Redirect)
            {
                // find all enemies on the other team within line of sight
                List<Transform> targets = new List<Transform>();
                foreach (Character ch in PowerArea.getAll())
                {
                    if (!ch.dead && ch.team != team)
                    {
                        if (ch.CanSee(prop))
                            targets.Add(ch.transform);
                    }
                }
                if (targets.Count > 0)
                {
                    newDir = (targets[UnityEngine.Random.Range(0, targets.Count)].position - position);
                    newDir.y = 0;
                    return newDir.normalized;
                }
            }

            // final fallthrough is Deflect (or redirect with no targets), pick a random direction within _+/-60 of the original direction
            Vector3 toSource = source - position;
            Vector3 sideWays = new Vector3(toSource.z, 0, -toSource.x);
            newDir = (toSource + UnityEngine.Random.Range(-2.0f, 2.0f) * sideWays);
            newDir.y = 0;
            return newDir.normalized;
        }

        public override string GetDescription(bool brief = false)
        {
            string msg = "";
            if (!brief)
                msg += name + "\n ";
            if (reflection != ReflectionType.None)
            {
                msg += "" + percentage + "% to " + reflection.ToString() + " " + damageType.ToString() + "\n ";
            }
            if (absorbFactor > 0 && absorbEffects.Length > 0)
            {
                msg +=  "" + percentage + "% to absorb " + damageType.ToString() + " to ";
                for (int i = 0; i < absorbEffects.Length; i++)
                {
                    msg += absorbEffects[i].name + ((i == absorbEffects.Length - 1) ? "\n " : ", ");
                }
            }

            return msg;
        }

        public void PlayEffect(Prop p)
        {
            responseFX.Begin(p.GetBodyPart(responseBodyPart), tint);
        }
    }
}
