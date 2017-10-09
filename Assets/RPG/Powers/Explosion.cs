using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "Explosion", menuName = "RPG/Status/Explosion", order = 1)]
    public class Explosion : Status
    {
        [Header("Explosion Settings")]
        public PowerArea explosion;

        // data to stop the same character exploding too regularly
        Dictionary<Prop, float> delays = new Dictionary<Prop, float>();

        public override void Apply(Prop ch, Character caster = null)
        {
            if (!delays.ContainsKey(ch) || delays[ch] < Time.time)
            {
                if (!explosion)
                {
                    Debug.Log("Explosion " + name + "has no PowerArea");
                    return;
                }
                if (explosion.userFX)
                {
                    GameObject go = explosion.userFX.Begin(ch.GetBodyPart(explosion.userBodyPart), explosion.tint);
                    explosion.userFX.ScaleToRadius(go, explosion.GetRadius(caster));
                }
                explosion.Explode(ch.transform, caster);
                delays[ch] = Time.time + 0.4f;
            }
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

        public override float BenefitPerHit(Character target)
        {
            return explosion.BenefitPerHit(target);
        }

        public override string GetDescription(bool brief = false)
        {
            return "Explosion ";
        }
    }
}
