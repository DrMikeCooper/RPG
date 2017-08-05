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

        public enum Mode
        {
            Instant,
            Charge,
            Maintain,
            Block
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
        public Mode mode = Mode.Instant;
        [Tooltip("How long the power lasts if its a Charge or Maintain")]
        public float duration;
        [Tooltip("Cost for full charge for a Charge, cost per tick for a Maintain")]
        public float extraEnergyCost;
        float timer;
        float nextTick;
        float tick = 0.5f; // expose this later?

        public Sprite icon;
        public RPGSettings.ColorCode color;

        public Animations animation = Animations.PunchRight;
        public bool colorParticles = false;
        public ParticleSystem userParticles;
        public Character.BodyPart userBodyPart = Character.BodyPart.RightHand;
        public ParticleSystem targetParticles;
        public Character.BodyPart targetBodyPart = Character.BodyPart.Chest;

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

        protected void StartPower(Character caster)
        {
            caster.PlayAnim(animation.ToString());
            caster.energy -= energyCost;
            AddParticles(userParticles, caster, userBodyPart);

            // turn to look at target but stay horizontal
            if (targetType != TargetType.SelfOnly && caster.target && caster.target != caster)
            {
                caster.transform.LookAt(caster.target.transform.position);
                caster.transform.eulerAngles = new Vector3(0, caster.transform.eulerAngles.y, 0);
            }
        }

        protected void EndPower(Character caster)
        {
            caster.activePower = null;
            caster.ReleaseAnim(true);

            // start the cool down
            caster.UsePower(this);
        }

        // apply this power to a particular target
        // charge varies from 0 to 1
        public void Apply(Character target, float charge)
        {
            // how does the damage get factored in?
            float damage;
            switch (mode)
            {
                case Mode.Charge:
                    damage = minDamage + charge * (maxDamage - minDamage);
                    break;
                case Mode.Maintain:
                    damage = minDamage + (1 - charge) * (maxDamage - minDamage);
                    break;
                default:
                    damage = Random.Range(minDamage, maxDamage);
                    break;
            }
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
                go.gameObject.name = ps.gameObject.name;
                // make sure there's a lifespan on the particle effect
                if (go.GetComponent<LifeSpan>() == null)
                    go.AddComponent<LifeSpan>().lifespan = 5;
                if (colorParticles)
                    go.GetComponent<ParticleSystem>().startColor = RPGSettings.GetColor(color);
            }
        }

        public void OnStart(Character caster)
        {
            if (CanUse(caster) == false)
                return;

            caster.activePower = this;
            StartPower(caster);
            switch (mode)
            {
                case Mode.Instant:
                    break;
                case Mode.Charge:
                    break;
                case Mode.Maintain:
                    nextTick = 0;
                    break;
                case Mode.Block:
                    break;
            }
            timer = 0;
        }

        public void OnUpdate(Character caster)
        {
            timer += Time.deltaTime;
            switch (mode)
            {
                case Mode.Instant:
                    break;
                case Mode.Charge:
                    {
                        float charge = (100.0f * timer) / duration;
                        if (charge > 100)
                        {
                            charge = 100;
                        }
                        caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue = charge;
                        // if we charge to full, finish
                        if (charge >= 100)
                        {
                            OnEnd(caster);
                            return;
                        }

                        // subtract energy for the charge and discharge if we run out
                        float deltaEnergy = Time.deltaTime * extraEnergyCost / duration;
                        caster.energy -= deltaEnergy;
                        if (caster.energy == 0)
                            OnEnd(caster);
                        break;
                    }
                case Mode.Maintain:
                    {
                        float charge = 100.0f * (1.0f - (timer / duration));
                        if (charge <= 0)
                        {
                            OnEnd(caster);
                            return;
                        }
                        caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue = charge;

                        // activate the power effect every tick
                        if (timer > nextTick)
                        {
                            nextTick += tick;
                            OnActivate(caster);

                            // subtract energy for the charge and discharge if we run out
                            float deltaEnergy = extraEnergyCost * tick/(duration);
                            caster.energy -= deltaEnergy;
                            if (caster.energy == 0)
                                OnEnd(caster);
                        }
                        break;
                    }
                case Mode.Block:
                    break;
            }
        }

        public void OnEnd(Character caster)
        {
            EndPower(caster);

            switch (mode)
            {
                case Mode.Instant:
                    OnActivate(caster);
                    break;
                case Mode.Charge:
                    OnActivate(caster);
                    caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue = 0;
                    break;
                case Mode.Maintain:
                    caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue = 0;
                    break;
                case Mode.Block:
                    break;
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

        // how many seconds will it take us to kill this character?
        protected float timeToDeath(Character caster, Character target)
        {
            float recharge = 3;
            float time = 0;
            float speed = 5;

            // time to get in range
            float distance = Vector3.Distance(caster.transform.position, target.transform.position);
            if (distance > range)
                time += distance / speed;

            // plaus time to kill
            if (maxDamage > 0)
            {
                time += target.health * recharge / ((minDamage + maxDamage) * 0.5f);
            }
            else
                time += 10.0f; // assume non-damage powers do something better else?

            return time;
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
