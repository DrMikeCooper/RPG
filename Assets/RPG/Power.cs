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

        public enum Animations
        {
            KickLeft,
            KickRight,
            PunchLeft,
            PunchRight
        };
    
        public float energyCost;
        public float coolDown;
        public float range;
        public RPGSettings.DamageType type;
        public TargetType targetType;

        public Sprite icon;
        public RPGSettings.ColorCode color;

        public Animations animation = Animations.PunchRight;
        public bool colorParticles = false;
        public ParticleSystem userParticles;
        public Character.BodyPart userBodyPart = Character.BodyPart.RightHand;
        public ParticleSystem targetParticles;
        public Character.BodyPart targetBodyPart = Character.BodyPart.Chest;

        // TODO animation for the power

        public float minDamage;
        public float maxDamage;
        public Status[] effects;
        public float statusDuration = 10;

        public bool CanUse(Character caster)
        {
            if (caster.GetCoolDown(this) > 0)
                return false;

            if (caster.energy < energyCost)
                return false;

            if (range > 0 && caster.target != null && Vector3.Distance(caster.transform.position, caster.target.transform.position) > range)
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
                if ((targetType == TargetType.SelfAndAllies || targetType == TargetType.All) && caster.target != null) 
                {
                    target = caster.target;
                    if (targetType == TargetType.SelfAndAllies && target && caster.team != target.team)
                        target = null;
                }
            }
            else
            {
                target = caster.target;
                if (target && target.team == caster.team)
                    target = null;
            }

            return target;
        }

        protected void UsePower(Character caster)
        {
            Animator animator = caster.GetComponent<Animator>();
            animator.Play(animation.ToString());
            

            caster.UsePower(this);
            AddParticles(userParticles, caster, userBodyPart);
            if (targetType != TargetType.SelfOnly && caster.target && caster.target != caster)
            {
                caster.transform.LookAt(caster.target.transform.position);
                caster.transform.eulerAngles = new Vector3(0, caster.transform.eulerAngles.y, 0);
            }
        }

        // apply this power to a particular target
        public void Apply(Character target)
        {
            float damage = Random.Range(minDamage, maxDamage);
            if (damage != 0)
                target.ApplyDamage(damage, type);
            foreach (Status s in effects)
                target.ApplyStatus(s, statusDuration);

            // particles on target
            AddParticles(targetParticles, target, targetBodyPart);
        }

        public void AddParticles(ParticleSystem ps, Character ch, Character.BodyPart bodyPart)
        {
            // particles on target
            if (ps != null)
            {
                GameObject go = Instantiate(ps.gameObject);
                go.transform.parent = ch.GetBodyPart(bodyPart);
                go.transform.localPosition = Vector3.zero;
                // make sure there's a lifespan on the particle effect
                if (go.GetComponent<LifeSpan>() == null)
                    go.AddComponent<LifeSpan>().lifespan = 5;
                if (colorParticles)
                    go.GetComponent<ParticleSystem>().startColor = RPGSettings.GetColor(color);
            }
        }

        public abstract void OnActivate(Character caster);

        // utility stuff for storing all characters. TODO Review this.
        static Character[] allCharacters;
        protected static Character[] getAll()
        {
            if (allCharacters == null)
                allCharacters = FindObjectsOfType<Character>();
            return allCharacters;
        }

        [HideInInspector]
        public Character npcTarget;

        // AI Action functions
        public virtual float Evaluate(NPCPowers npc) { return 0; }
        public virtual void Enter(NPCPowers npc) { }
        public virtual void Exit(NPCPowers npc) { }
        public virtual void UpdateAction(NPCPowers npc) { }
    }
}
