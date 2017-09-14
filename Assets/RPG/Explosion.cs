﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "Explosion", menuName = "RPG/Explosion", order = 1)]
    public class Explosion : Status
    {
        public PowerArea explosion;

        public override void Apply(Prop ch)
        {
            if (!explosion)
            {
                Debug.Log("Explosion " + name + "has no PowerArea");
                return;
            }
            if (explosion.userFX)
                explosion.userFX.Begin(ch.GetBodyPart(explosion.userBodyPart), explosion.tint);
            explosion.Explode(ch.transform, null);
        }

        public override bool isImmediate() { return true; }

        public override float DamagePerHit()
        {
            return explosion.DamagePerHit();
        }

        public override float StatusPerHit(Character target)
        {
            return explosion.StatusPerHit(target);
        }

        public virtual float BenefitPerHit(Character target)
        {
            return explosion.BenefitPerHit(target);
        }
    }
}
