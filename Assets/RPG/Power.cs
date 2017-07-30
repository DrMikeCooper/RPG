using System.Collections;
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
        public RPGSettings.DamageType type;
        public TargetType targetType;

        public Sprite icon;
        public Color color = Color.white;

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

        public abstract void OnActivate(Character caster);

    }
}
