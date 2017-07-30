﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerDirect", menuName = "RPG/PowerDirect", order = 1)]
    public class PowerDirect : Power
    {
        // useable on a single target with no to-hit roll
        public override void OnActivate(Character caster)
        {
            // check standard cooldown, energy cost etc
            if (!CanUse(caster))
                return;

            Character target = GetTarget(caster);

            if (target)
            {
                caster.UsePower(this);
                float damage = Random.Range(minDamage, maxDamage);
                if (damage != 0)
                    target.ApplyDamage(damage, type);
                foreach (Status s in effects)
                    target.ApplyStatus(s);
            }
        }
    }
}