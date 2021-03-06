﻿using System.Collections;
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
            All,
            DeadAllies,
            DeadEnemies,
        };

        public enum Mode
        {
            Instant,
            MoveTo,
            Maintain,
            Charge,
            Block,
        };

        public enum ApplyStatusMaintains
        {
            FinalTick,
            FirstTick,
            EveryTick,
        };
    
        public enum Animations
        {
            KickLeft,
            KickRight,
            PunchLeft,
            PunchRight,
            axe_kick,
            back_kick,
            brazilian_kick,
            defensive_side_kick,
            front_kick,
            CombatReady,
            None
        };
    
        [Header("General Settings")]
        public float energyCost;
        public float coolDown;
        [ShowIf("targetType", ShowIfAttribute.Comparison.Not, (int)TargetType.SelfOnly)]
        public float range;
        [ShowIf("targetType", ShowIfAttribute.Comparison.Not, (int)TargetType.SelfOnly)]
        public bool lineOfSight = true;
        public RPGSettings.DamageType type;
        public TargetType targetType;
        [HideIfType(typeof(PowerToggle))]
        public Mode mode = Mode.Instant;
        [ShowIf("mode", ShowIfAttribute.Comparison.Greater, (int)Mode.MoveTo)]
        [Tooltip("How long the power lasts if its a Charge or Maintain")]
        public float duration;
        [ShowIf("mode", ShowIfAttribute.Comparison.Greater, (int)Mode.MoveTo)]
        [Tooltip("Cost for full charge for a Charge, cost per tick for a Maintain")]
        public float extraEnergyCost;
        [ShowIf("mode", ShowIfAttribute.Comparison.Greater, (int)Mode.MoveTo)]
        public bool interruptable = false;
        [ShowIf("mode", ShowIfAttribute.Comparison.Equals, (int)Mode.Maintain)]
        [Tooltip("Interval at which the power ticks, used for Maintains")]
        public float tick = 0.5f;
        [ShowIf("mode", ShowIfAttribute.Comparison.Equals, (int)Mode.Maintain)]
        public ApplyStatusMaintains applyStatusWhen;
        [ShowIf("mode", ShowIfAttribute.Comparison.Greater, (int)Mode.MoveTo)]
        public float maxRadius = 0;
        [ShowIf("mode", ShowIfAttribute.Comparison.Greater, (int)Mode.MoveTo)]
        public float maxStatusDuration = 0;
        [ShowIf("mode", ShowIfAttribute.Comparison.Greater, (int)Mode.MoveTo)]
        public float maxSelfStatusDuration = 0;
        [ShowIf("mode", ShowIfAttribute.Comparison.Greater, (int)Mode.MoveTo)]
        public float maxBuffStrength = 0;
        public PowerSounds sounds;

        [Header("HUD Settings")]
        public Sprite icon;
        public RPGSettings.Tint tint;

        [Header("Animation Settings")]
        public Animations animation = Animations.PunchRight;

        public VisualEffect userFX;
        public Character.BodyPart userBodyPart = Character.BodyPart.RightHand;
        [ShowIf("targetType", ShowIfAttribute.Comparison.Not, (int)TargetType.SelfOnly)]
        public VisualEffect targetFX;
        [ShowIf("targetType", ShowIfAttribute.Comparison.Not, (int)TargetType.SelfOnly)]
        public Character.BodyPart targetBodyPart = Character.BodyPart.Chest;

        [Header("RPG Effects")]
        [ShowIf("targetType", ShowIfAttribute.Comparison.Not, (int)TargetType.SelfOnly)]
        public float accuracy = 1;
        public float minDamage;
        public float maxDamage;
        public Status[] effects;
        public float statusDuration = 10;
        [ShowIf("targetType", ShowIfAttribute.Comparison.Not, (int)TargetType.SelfOnly)]
        public Status[] selfEffects;
        [ShowIf("targetType", ShowIfAttribute.Comparison.Not, (int)TargetType.SelfOnly)]
        public float selfStatusDuration = 10;

        [ShowIf("mode", ShowIfAttribute.Comparison.Equals, (int)Mode.MoveTo)]
        // the speed at which we lunge towards the target
        public float closeToTargetSpeed = 10;

        [HideInInspector]
        public bool lunge = false;

        // for charge and maintains - stores the user fx to keep going while charging/maintaining
        GameObject userFXInstance;

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
                if ((targetType == TargetType.AlliesOnly || targetType == TargetType.SelfAndAllies || targetType == TargetType.All) && caster.target != null) 
                {
                    target = caster.target;
                    Character ctarget = caster.target as Character;
                    if ((targetType == TargetType.AlliesOnly || targetType == TargetType.SelfAndAllies) && ctarget && caster.GetTeam() != ctarget.team)
                        target = null;
                }
            }
            else
            {
                target = caster.target;
                Character ctarget = target as Character;
                if (ctarget && ctarget.team == caster.GetTeam())
                    target = null;
            }

            return target;
        }

        public bool CanUseOn(Character caster, Prop target)
        {
            Character ctarget = target as Character;
            if (ctarget)
            {
                bool isSelf = ctarget == caster;
                bool isAlly = ctarget.team == caster.GetTeam();
                switch (targetType)
                {
                    case TargetType.AlliesOnly: return isAlly && !isSelf;
                    case TargetType.Enemies: return !isAlly;
                    case TargetType.SelfAndAllies: return isAlly;
                    case TargetType.SelfOnly: return isSelf;
                }
                return true;
            }
            else
                return targetType != TargetType.SelfOnly;
        }

        // does the animation, FX on caster, sound and uses activation energy
        protected void StartPower(Character caster)
        {
            if (sounds)
                sounds.PlayStart(caster.audioSource);
            caster.PlayAnim(animation.ToString());
            caster.energy -= energyCost;
            if (userFX)
            {
                bool continuous = (mode == Mode.Charge || mode == Mode.Maintain);
                GameObject fx = userFX.Begin(caster.GetBodyPart(userBodyPart), tint, !continuous, true);

                // scale up area powers to match the radius
                PowerArea area = this as PowerArea;
                if (area)
                    userFX.ScaleToRadius(fx, area.GetRadius(caster));

                if (continuous)
                    userFXInstance = fx;
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
            // close off any fx from charge/maintain
            if (userFXInstance)
            {
                userFX.End(userFXInstance);
                userFXInstance = null;
            }
            caster.ReleaseAnim(true);

            // start the cool down
            if (this as PowerToggle == null)
                caster.UsePower(this);
        }

        // apply this power to a particular target
        // charge varies from 0 to 1
        public bool Apply(Prop target, float charge, Character caster, bool doStatus = true)
        {
            // calculate chance to hit
            if (targetType == TargetType.Enemies)
            {
                float casterAcc = caster == null ? 0 : caster.stats[RPGSettings.StatName.Accuracy.ToString()].currentValue;
                float chanceToHit = RPGSettings.instance.baseAccuracy * accuracy + casterAcc - target.stats[RPGSettings.GetDefenceStat(type)].currentValue;
                if (Random.Range(0, 100) > chanceToHit)
                {
                    target.NumberFloat("MISS!", Color.white);
                    return false;
                }
            }

            Character ch = target as Character;
            if (ch && sounds)
                sounds.PlayHit(ch.audioSource);

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
            if (caster)
                damage *= caster.GetFactor(RPGSettings.GetDamageStat(type));

            if (damage != 0)
                target.ApplyDamage(damage, type);
            if (doStatus)
            {
                foreach (Status s in effects)
                    target.ApplyStatus(s, GetDuration(caster), caster, this);
                foreach (Status s in selfEffects)
                    caster.ApplyStatus(s, GetSelfDuration(caster), caster, this);

                // add to a global list of DoT's if not a character?
                if (target as Character == null)
                {
                    if (!Prop.activeProps.Contains(target.gameObject))
                        Prop.activeProps.Add(target.gameObject);
                }
            }

            // particles on target
            if (targetFX)
                targetFX.Begin(target.GetBodyPart(targetBodyPart), tint);

            // hit responses on the target
            foreach (HitResponse hr in target.hitResponses)
            {
                if (Random.Range(0, 100) < hr.percentage && (hr.damageType & type) != 0)
                    hr.OnHit(target, damage);
            }

            return true;
        }

        // called when key is pressed down
        // for Instant powers, this just sets activePower
        // otherwise calls StartPower, which fires of the animation
        public void OnStart(Character caster)
        {
            if (CanUse(caster) == false)
                return;

            // checks combos for advancement, or returns the base power
            caster.activePower = GetPower(caster);
            if (!IsInstant())
                caster.activePower.StartPower(caster);
            caster.nextTick = 0;

            if (mode == Mode.Block)
            {
                statusDuration = selfStatusDuration = tick + 0.5f;
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
                float dist = Vector3.Distance(caster.transform.position, caster.target.transform.position);
                if (dist <= 1 || caster.timer > 5) // 5 second limit to prevent you getting stuck in a lunge
                {
                    lunge = false;
                    OnEnd(caster);
                }
                return;
            }

            switch (mode)
            {

                case Mode.Instant:
                case Mode.MoveTo:
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
                            // figure out if we're applying statuses this frame or not
                            bool doStatus = true;
                            if (applyStatusWhen == ApplyStatusMaintains.FirstTick && caster.nextTick >= tick)
                                doStatus = false;

                            caster.nextTick += tick;

                            if (applyStatusWhen == ApplyStatusMaintains.FinalTick && caster.nextTick <= duration)
                                doStatus = false;

                            OnActivate(caster, doStatus);

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
                        Apply(caster, 1, caster);
                        //foreach (Status s in effects)
                        //{
                        //    Explosion ex = s as Explosion;
                        //    if (ex)
                        //        ex.Apply(caster);
                        //}
                    }
                    break;
            }
        }

        public void OnEnd(Character caster, bool interrupted = false)
        {
            if (IsInstant())
            {
                //Debug.Log("OnEnd: index = " + caster.currentComboStage);
                GetPower(caster).StartPower(caster);

                // set off a coroutine to advance combos
                PowerCombo combo = this as PowerCombo;
                if (combo)
                    caster.QueueAdvanceCombo(combo);

                if (mode == Mode.MoveTo)
                {
                    float dist = caster.target== null ? 0 : Vector3.Distance(caster.transform.position, caster.target.transform.position);
                    if (dist > 1)
                    {
                        lunge = true;
                        return;
                    }
                    else
                    {
                        lunge = false;
                        caster.Hit();
                    }
                }
            }
            else
                caster.activePower = null;

            caster.EnableAnimator(2);

            if (mode != Mode.MoveTo || lunge == false)
                EndPower(caster);

            switch (mode)
            {
                case Mode.Instant:
                case Mode.MoveTo:
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

        public abstract void OnActivate(Character caster, bool doStatus = true);

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

        // activate the power using AI
        protected void OnDoPower(AIBrain brain)
        {
            Character caster = brain.character;

            brain.MoveTo(caster.transform);
            OnStart(caster);

            // instant powers release the key immediately. Others, we let energy or duration end it.
            if (IsInstant())
                OnEnd(caster);
        }

        public float EvaluateRanged(AIBrain brain, AINode.AICondition condition)
        {
            Character caster = brain.character;

            npcTarget = null;
            float bestEval = 0;
            List<Character> targets = targetType == TargetType.Enemies ? brain.enemies : brain.allies;
            foreach (Character ch in targets)
            {
                if (ch != caster)
                {
                    bool cond = AINode.IsCondition(condition, ch, this);
                    if (cond)
                    {
                        float eval = Eval(caster, ch);

                        // TODO - factor in collateral and splash damage ?

                        //Debug.Log("Power:" + name + " Ch:" + ch.name + " Eval:" + eval);
                        if (eval > bestEval)
                        {
                            bestEval = eval;
                            npcTarget = ch;
                        }
                    }
                   // else
                        //Debug.Log("Power:" + name + " Ch:" + ch.name + "condition not met");
                }
            }

            return bestEval;
        }

        float Eval(Character caster, Character target)
        {
            float eval = 0;
            if (targetType == TargetType.Enemies)
            {
                if (target != caster && target.team != caster.GetTeam())
                {
                    eval = 100.0f / timeToDeath(caster, target);
                }
            }
            else // ally or self benefit power
            {
                if (target.team == caster.GetTeam() && !(target == caster && targetType == TargetType.AlliesOnly) && !(target != caster && targetType == TargetType.SelfOnly))
                {
                    eval = BenefitPerHit(target);
                }
            }
            //target.AddAIDebugText(caster, name + " " + eval);
            caster.AddAIDebugText(caster, "-PW(" + eval + ")" + name + " vs. " + target.name);
            return eval;
        }

        public float DamagePerHit()
        {
            float damage = (minDamage + maxDamage)*0.5f;

            for (int i = 0; i < effects.Length; i++)
            {
                damage += effects[i].DamagePerHit(); 
            }

            return damage;
        }

        public float StatusPerHit(Character target)
        {
            float value = 0;

            for (int i = 0; i < effects.Length; i++)
            {
                value += effects[i].StatusPerHit(target);
            }

            return value;
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

            // this equates status to damage to attempt to work out time to death of the target
            time += target.health * recharge / (StatusPerHit(target) + DamagePerHit());

            return time;
        }

        // figure out how much a firendly power helps the target
        public float BenefitPerHit(Character target)
        {
            float benefit = 0;

            // if its a heal - either negative damage or DoT
            float heal = -DamagePerHit();
            float damage = target.maxHealth - target.health;
            heal = Mathf.Min(heal, damage); // take into account over-heal
            float pct = target.GetHealthPct();
            // weight this for characters low on health = 1 at half health, 4 at death's door
            float urgency = 4.0f * (1.0f - pct) * (1.0f - pct);

            benefit += urgency * heal;

            // status effect benefits
            for (int i = 0; i < effects.Length; i++)
            {
                benefit += effects[i].BenefitPerHit(target);
            }

            return benefit;
        }


        // AI Behaviour interface implemented directly by powers
        public override float Evaluate(AIBrain brain)
        {
            // return zero if the power's on cooldown or we don't have enough energy
            if (brain.character.GetCoolDown(this) > 0 || energyCost > brain.character.energy)
                return 0;

            return Evaluate(brain, AINode.always);
        }

        public override AIAction Execute(AIBrain brain)
        {
            if (npcTarget == null)
                Evaluate(brain);

            brain.target = npcTarget;
            //Debug.Log("Executing Power: " + name + " Ch:" + npcTarget.name);
            Character caster = brain.character;
            caster.target = brain.target;

            if (!WithinRange(caster))
            {
                brain.MoveTo(caster.target.transform);
                brain.countDown = 0.5f;
                brain.closingRange = range;
                return null;
            }

            if (CanUse(brain.character))
                UpdateAction(brain);

            return this;
        }

        public override float GetDuration()
        {
            return 3 + this.duration;
        }

        public string GetDescription(bool skipName = false)
        {
            string desc = skipName? "" : name;
            if (mode == Mode.Charge) desc += "(Charge)";
            if (mode == Mode.Maintain) desc += "(Maintain)";
            if (mode == Mode.Block) desc += "(Block)";
            desc += "\n ";

            if (minDamage + maxDamage > 0)
            {
                if (minDamage == maxDamage)
                    desc += minDamage + " " + type.ToString() + " damage\n ";
                else
                    desc += minDamage + "-" + maxDamage + " " + type.ToString() + " damage\n ";
            }
            else if (minDamage + maxDamage < 0)
            {
                if (minDamage == maxDamage)
                    desc += (-minDamage) + " heal\n ";
                else
                    desc += (-minDamage) + "-" + (-maxDamage) + " heal\n ";
            }
            for (int k = 0; k < effects.Length; k++)
            {
                desc += effects[k].GetDescription(true) + ((k != effects.Length - 1) ? ", " : "\n ");
            }
            for (int k = 0; k < selfEffects.Length; k++)
            {
                desc += selfEffects[k].GetDescription(true) + ((k != selfEffects.Length - 1) ? ", " : " to self\n ");
            }

            return desc;
        }

        public virtual Power GetStartPower(Character caster)
        {
            caster.ResetCombos();
            return this;
        }

        public virtual Power GetPower(Character caster)
        {
            return this;
        }

        public bool IsInstant()
        {
            return mode == Mode.Instant || mode == Mode.MoveTo;
        }

        protected void AddBeamBetween(Character source, Character dest, Character.BodyPart sourceBodyPart, PowerBeam.BeamSettings settings)
        {
            GameObject beamObj = RPGSettings.instance.beamPool.GetObject();
            if (beamObj)
            {
                BeamRenderer beam = beamObj.GetComponent<BeamRenderer>();
                beam.Activate(source.GetBodyPart(sourceBodyPart), dest.GetBodyPart(targetBodyPart), settings, tint.GetColor());
            }
            else
                Debug.Log("Running out of beams! Increase the pool size in RPGSettings");
        }

        public float GetDuration(Character caster)
        {
            float d = statusDuration;
            if (maxStatusDuration != 0 && caster)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                d *= charge * maxStatusDuration + (1 - charge) * statusDuration;
            }
            return d;
        }

        public float GetSelfDuration(Character caster)
        {
            float d = statusDuration;
            if (maxSelfStatusDuration != 0 && caster)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                d *= charge * maxSelfStatusDuration + (1 - charge) * selfStatusDuration;
            }
            return d;
        }

        public float GetBuffPower(Character caster, float basePower)
        {
            float p = basePower;
            if (maxBuffStrength != 0 && caster)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                p *= charge * maxBuffStrength + (1 - charge) * basePower;
            }
            return p;
        }

        #region Editor Utilities

        protected void SetIcon(string iconName)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/" + iconName);
            }
#endif
        }

        protected T LoadAsset<T>(string assetName) where T : ScriptableObject
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>("Assets/" + assetName);
            }
#endif
            return null;
        }

        // adds this status to our list if it isnt already in there
        protected void AddStatus(Status s)
        {
            if (s == null)
                return;

            for (int i = 0; i < effects.Length; i++)
                if (effects[i] == s)
                    return;
            Status[] newEffects = new Status[effects.Length + 1];
            for (int i = 0; i < effects.Length; i++)
                newEffects[i] = effects[i];
            newEffects[effects.Length] = s;
            effects = newEffects;
        }

        [ContextMenu("Fire Settings")]
        protected void MakeFire()
        {
            tint.code = RPGSettings.ColorCode.Fire;
            type = RPGSettings.DamageType.Fire;
            AddStatus(LoadAsset<Status>("Sample Powers/Flames.asset"));
        }

        #endregion

    }
}
