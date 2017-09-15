﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "Knockback", menuName = "RPG/Knockback", order = 1)]
    public class Knockback : Status
    {
        [Range(1,5)]
        public float strength;
        public bool knockUp;

        public override void Apply(Prop ch, Character caster = null)
        {
            Vector3 force;
            if (knockUp)
            {
                force = Vector3.up;
            }
            else
            {
                force = (ch.transform.position - caster.transform.position).normalized;
                force.y = 1;
            }
            ch.ApplyKnockback(strength * 250 * force);
        }

        public override bool isImmediate() { return true; }

        public override float DamagePerHit()
        {
            return strength;
        }

        public override float StatusPerHit(Character target)
        {
            return 50;
        }

        public override float BenefitPerHit(Character target)
        {
            return -50;
        }
    }
}
