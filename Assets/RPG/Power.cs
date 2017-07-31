﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public abstract class Power : ScriptableObject
    {
        public enum TargetType
        {
            SelfOnly,
            SelfAndAllies,
            AlliesOnly,
            Enemies,
            All
        };

        public float energyCost;
        public float coolDown;
        public float range;
        public RPGSettings.DamageType type;
        public TargetType targetType;

        public Sprite icon;
        public RPGSettings.ColorCode color;

        public bool colorParticles = false;
        public ParticleSystem userParticles;
        public ParticleSystem targetParticles;

        // TODO animation for the power

        public float minDamage;
        public float maxDamage;
        public Status[] effects;

        public bool CanUse(Character caster)
        {
            if (caster.GetCoolDown(this) > 0)
                return false;

            if (caster.energy < energyCost)
                return false;

            if (range > 0 && targetParticles != null && Vector3.Distance(caster.transform.position, targetParticles.transform.position) > range)
                return false;

            return true;
        }

        public Character GetTarget(Character caster)
        {
            Character target = null;
            if (targetType != TargetType.Enemies)
            {
                target = caster;
                if (targetType == TargetType.AlliesOnly)
                    target = null;
                if ((targetType == TargetType.SelfAndAllies || targetType == TargetType.All) && caster.target != null) // TODO check alignment
                {
                    target = caster.target;
                }
            }
            else
            {
                // TODO - check alignment
                target = caster.target;
            }

            return target;
        }

        protected void UsePower(Character caster)
        {
            caster.UsePower(this);
            AddParticles(userParticles, caster);
            if (targetType!= TargetType.SelfOnly && caster.target && caster.target != caster)
                caster.transform.LookAt(caster.target.transform.position);
        }

        // apply this power to a particular target
        protected void Apply(Character target)
        {
            float damage = Random.Range(minDamage, maxDamage);
            if (damage != 0)
                target.ApplyDamage(damage, type);
            foreach (Status s in effects)
                target.ApplyStatus(s);

            // particles on target
            AddParticles(targetParticles, target);
        }

        public void AddParticles(ParticleSystem ps, Character ch)
        {
            // particles on target
            if (ps != null)
            {
                GameObject go = Instantiate(ps.gameObject);
                go.transform.parent = ch.transform;
                go.transform.position = ch.transform.position;
                // make sure there's a lifespan on the particle effect
                if (go.GetComponent<LifeSpan>() == null)
                    go.AddComponent<LifeSpan>().lifespan = 5;
                if (colorParticles)
                    go.GetComponent<ParticleSystem>().startColor = RPGSettings.instance.GetColor(color);
            }
        }

        public abstract void OnActivate(Character caster);

    }
}
