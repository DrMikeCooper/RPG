using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "HitResponse", menuName = "RPG/HitResponse", order = 2)]
    public class HitResponse : Status
    {
        [Tooltip("Chance of this response triggering when hit")]
        public int percentage;

        [Tooltip("Tick to deflect beams and projectiles")]
        public bool reflection = false;

        [Tooltip("Damage multiplied by this number goes to the named stat")]
        public float absorbFactor = 0;
        [Tooltip("An effect that plays when you absorb damage")]
        public Status absorbEffect;

        public override void Apply(Prop ch)
        {
            ch.hitResponses.Add(this);
        }

        public void OnHit(Prop prop, float damage)
        {
            if (UnityEngine.Random.Range(0, 100) > percentage)
                return;

            if (absorbFactor > 0)
            {
                prop.ApplyStatus(absorbEffect, damage * absorbFactor);
            }
        }
    }
}
