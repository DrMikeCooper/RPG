using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public abstract class Power : AIAction
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
            Block,
        };
    
        public enum Animations
        {
            KickLeft,
            KickRight,
            PunchLeft,
            PunchRight,
            None
        };
    
        public float energyCost;
        public float coolDown;
        public float range;
        public bool lineOfSight = true;
        public RPGSettings.DamageType type;
        public TargetType targetType;
        public Mode mode = Mode.Instant;
        [Tooltip("How long the power lasts if its a Charge or Maintain")]
        public float duration;
        [Tooltip("Cost for full charge for a Charge, cost per tick for a Maintain")]
        public float extraEnergyCost;
        public bool interruptable = false;

        [Tooltip("Interval at which the power ticks, used for Maintains")]
        public float tick = 0.5f; 

        public Sprite icon;
        public RPGSettings.ColorCode color;

        public Animations animation = Animations.PunchRight;
        public Animations releaseAnimation = Animations.None;

        public VisualEffect userFX;
        public Character.BodyPart userBodyPart = Character.BodyPart.RightHand;
        public VisualEffect targetFX;
        public Character.BodyPart targetBodyPart = Character.BodyPart.Chest;

        public float minDamage;
        public float maxDamage;
        public Status[] effects;
        public float statusDuration = 10;

        // true of we run to the target in order to get within 1 unit before activating
        public float closeToTargetSpeed = 0;

        bool lunge = false;

        public bool CanUse(Character caster)
        {
            if (caster.isHeld())
                return false;

            if (caster.GetCoolDown(this) > 0)
                return false;

            if (caster.energy < energyCost)
                return false;

            return targetType == TargetType.SelfOnly || WithinRange(caster);
        }

        public bool WithinRange(Character caster)
        {
            return Power.WithinRange(caster, range, lineOfSight);
        }

        public static bool WithinRange(Character caster, float range, bool lineOfSight)
        {
            if (caster.target != null && caster.target!=caster)
            {
                // just too far away as the crow flies
                if (range > 0 && Vector3.Distance(caster.transform.position, caster.target.transform.position) > range)
                    return false;

                // can't see the target and LoS required
                if (lineOfSight && caster.CanSee(caster.target) == false)
                    return false;
            }

            return true;
        }

        public Prop GetTarget(Character caster)
        {
            Prop target = null;
            if (targetType != TargetType.Enemies)
            {
                target = caster;
                if (targetType == TargetType.AlliesOnly)
                    target = null;
                if ((targetType == TargetType.SelfAndAllies || targetType == TargetType.All) && caster.target != null) 
                {
                    Character ctarget = caster.target as Character;
                    if (targetType == TargetType.SelfAndAllies && ctarget && caster.team != ctarget.team)
                        target = null;
                }
            }
            else
            {
                target = caster.target;
                Character ctarget = target as Character;
                if (ctarget && ctarget.team == caster.team)
                    target = null;
            }

            return target;
        }

        protected void StartPower(Character caster)
        {
            caster.PlayAnim(animation.ToString());
            caster.energy -= energyCost;
            if (userFX)
            {
                userFX.Begin(caster.GetBodyPart(userBodyPart), color);
            }

            FaceTarget(caster);
        }

        protected void FaceTarget(Character caster)
        {
            // turn to look at target but stay horizontal
            if (targetType != TargetType.SelfOnly && caster.target && caster.target != caster)
            {
                caster.FaceTarget();
            }
        }

        protected void EndPower(Character caster)
        {
            caster.ReleaseAnim(true);

            // start the cool down
            if (this as PowerToggle == null)
                caster.UsePower(this);
        }

        // apply this power to a particular target
        // charge varies from 0 to 1
        public void Apply(Prop target, float charge)
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

            // add to a global list of DoT's if not a character?
            if (target as Character == null)
            {
                if (!Prop.activeProps.Contains(target.gameObject))
                    Prop.activeProps.Add(target.gameObject);
            }

            // particles on target
            if (targetFX)
                targetFX.Begin(target.GetBodyPart(targetBodyPart), color);

            // hit responses on the target
            foreach (HitResponse hr in target.hitResponses)
            {
                if (Random.Range(0, 100) < hr.percentage && (hr.damageType & type) != 0)
                    hr.OnHit(target, damage);
            }
        }

        public void OnStart(Character caster)
        {
            if (CanUse(caster) == false)
                return;

            caster.activePower = this;
            if (mode != Mode.Instant)
                StartPower(caster);
            caster.nextTick = 0;

            if (mode == Mode.Block)
            {
                caster.statusDirty = true;
                caster.ReleaseAnim(true);
            }

            caster.timer = 0;
        }

        public void OnUpdate(Character caster)
        {
            if (caster.powerStarted)
                caster.timer += Time.deltaTime;

            // special code once a lunge is triggered
            if (lunge)
            {
                FaceTarget(caster);
                caster.transform.position = Vector3.MoveTowards(caster.transform.position, caster.target.transform.position, closeToTargetSpeed*Time.deltaTime);
                if (Vector3.Distance(caster.transform.position, caster.target.transform.position) <= 1)
                    OnEnd(caster);
                return;
            }

            switch (mode)
            {
                case Mode.Instant:
                    break;
                case Mode.Charge:
                    {
                        float charge = (100.0f * caster.timer) / duration;
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
                        if (caster.isHeld())
                            OnEnd(caster);
                        FaceTarget(caster);
                        break;
                    }
                case Mode.Maintain:
                    {
                        float charge = 100.0f * (1.0f - (caster.timer / duration));
                        
                        caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue = charge;

                        // activate the power effect every tick
                        if (caster.timer >= caster.nextTick)
                        {
                            caster.nextTick += tick;
                            OnActivate(caster);

                            // subtract energy for the charge and discharge if we run out
                            float deltaEnergy = extraEnergyCost * tick/(duration);
                            caster.energy -= deltaEnergy;
                            if (caster.energy == 0)
                                OnEnd(caster);
                        }
                        if (charge <= 0)
                        {
                            OnEnd(caster);
                            return;
                        }
                        if (caster.isHeld())
                            OnEnd(caster, true);
                        FaceTarget(caster);
                        break;
                    }
                case Mode.Block:
                    caster.energy -= energyCost * Time.deltaTime;
                    if (caster.energy == 0)
                        OnEnd(caster);
                    if (caster.timer >= caster.nextTick)
                    {
                        caster.nextTick += tick;
                        foreach (Status s in effects)
                        {
                            Explosion ex = s as Explosion;
                            if (ex)
                                ex.Apply(caster);
                        }
                    }
                    break;
            }
        }

        public void OnEnd(Character caster, bool interrupted = false)
        {
            if (mode == Mode.Instant)
            {
                StartPower(caster);
                if (closeToTargetSpeed > 0 && Vector3.Distance(caster.transform.position, caster.target.transform.position) > 1)
                {
                    lunge = true;
                    return;
                }
            }
            else
                caster.activePower = null;

            if (releaseAnimation != Animations.None)
                caster.PlayAnim(releaseAnimation.ToString());

            EndPower(caster);
            switch (mode)
            {
                case Mode.Instant:
                    break;
                case Mode.Charge:
                    if (!interrupted)
                    {
                        OnActivate(caster);
                    }
                    caster.activePower = null;
                    caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue = 0;
                    break;
                case Mode.Maintain:
                    caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue = 0;
                    break;
                case Mode.Block:
                    caster.statusDirty = true;
                    break;
            }
        }

        public abstract void OnActivate(Character caster);

        // utility stuff for storing all characters. TODO Review this.
        static Character[] allCharacters;

        public static void ClearCharacterList()
        {
            allCharacters = null;
        }

        public static Character[] getAll()
        {
            if (allCharacters == null)
                allCharacters = FindObjectsOfType<Character>();
            return allCharacters;
        }

        

        [HideInInspector]
        public Character npcTarget;

        // AI Action functions
        public virtual float Evaluate(AIBrain brain, AINode.AICondition condition) { return 0; }
        public virtual void Enter(AIBrain brain) { }
        public virtual void Exit(AIBrain brain) { }
        public virtual void UpdateAction(AIBrain brain) { }

        // AI Utility functions
        protected void OnUpdateRanged(AIBrain brain)
        {
            Character caster = brain.character;
            Character target = brain.target;
            caster.target = target;

            if (target == null)
                return;

            if (!WithinRange(caster))
            {
                brain.MoveTo(target.transform);
                brain.countDown = 1.0f;
                brain.closingRange = range;
            }
            else
            {
                OnDoPower(brain);
            }
        }

        protected void OnDoPower(AIBrain brain)
        {
            Character caster = brain.character;

            brain.MoveTo(caster.transform);
            OnStart(caster);

            // instant powers release the key immediately. Others, we let energy or duration end it.
            if (mode == Mode.Instant)
                OnEnd(caster);
        }

        public float EvaluateRanged(AIBrain brain, AINode.AICondition condition)
        {
            Character caster = brain.character;

            npcTarget = null;
            float bestEval = 0;
            foreach (Character ch in brain.enemies)
            {
                if (ch != caster && ch.team != caster.team && AINode.IsCondition(condition, ch, this))
                {
                    float eval = 100.0f / timeToDeath(caster, ch);

                    // TODO - factor in collateral and splash damage ?

                    if (eval > bestEval)
                    {
                        bestEval = eval;
                        npcTarget = ch;
                    }
                }
            }

            return bestEval;
        }

        // how many seconds will it take us to kill this character?
        protected float timeToDeath(Character caster, Character target)
        {
            float recharge = 3;
            float time = 0;
            float speed = 5;

            // time to get in range TODO pathfind?
            float distance = Vector3.Distance(caster.transform.position, target.transform.position);
            if (distance > range)
                time += distance / speed;

            // plus time to kill
            if (maxDamage > 0)
            {
                time += target.health * recharge / ((minDamage + maxDamage) * 0.5f);
            }
            else
                time += 10.0f; // assume non-damage powers do something better else?

            return time;
        }

        // AI Behaviour interface implemented directly by powers
        public override float Evaluate(AIBrain brain)
        {
            return Evaluate(brain, AINode.AICondition.Always);
        }

        public override AIAction Execute(AIBrain brain)
        {
            Evaluate(brain);
            brain.target = npcTarget;

            UpdateAction(brain);

            return this;
        }

        public override float GetDuration()
        {
            return 3 + this.duration;
        }
    }
}
