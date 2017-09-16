using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace RPG
{
    [RequireComponent(typeof(CharacterParticles))]
    // destructable prop with health and a possible explosion
    public class Prop : MonoBehaviour
    {
        public float maxHealth = 100;
        protected Stat healthStat;

        public string characterName;
        public Sprite portrait;
        [HideInInspector]
        public Prop target; // who is this character targetting (can be null or yourself)?
        
        [Tooltip("Explosion power that plays on destruction")]
        public PowerArea explosion;

        public Status[] passives;

        [HideInInspector]
        public bool dead = false;

        bool knockback = false;

        public float health
        {
            get
            {
                //Some other code
                return healthStat.baseValue;
            }
            set
            {
                //Some other code
                healthStat.baseValue = healthStat.currentValue = Mathf.Clamp(value, 0, maxHealth);
            }
        }

        [HideInInspector]
        public List<Status> statusEffects = new List<Status>();
        [HideInInspector]
        public Dictionary<string, Status> groupedEffects = new Dictionary<string, Status>();

        [HideInInspector]
        public bool statusDirty;
       

        [HideInInspector]
        public Dictionary<string, Stat> stats = new Dictionary<string, Stat>();

        // here in the base class for logic in ProcessStatus, only ever non-null on characters
        [HideInInspector]
        public Power activePower;

        public List<HitResponse> hitResponses = new List<HitResponse>();

        protected float fadeTime = 0.5f;

        // global list of atcive props to receive updates for DoTs etc
        public static List<GameObject> activeProps = new List<GameObject>();

        [System.Serializable]
        public class DeathEvent : UnityEvent<Prop>
        {
        }

        [System.Serializable]
        public class DamageEvent : UnityEvent<Prop, int>
        {
        }

        [System.Serializable]
        public class StatusEvent : UnityEvent<Prop, Status>
        {
        }

        public DeathEvent onDead = new DeathEvent();
        public DamageEvent onDamaged = new DamageEvent();
        public StatusEvent onStatusChanged = new StatusEvent();

        // Use this for initialization
        void Start()
        {
            InitProp();
            ApplyPassives();
        }

        protected void InitProp()
        {
            statusDirty = true;

            for (int i = 0; i < RPGSettings.BasicDamageTypesCount; i++)
            {
                stats[RPGSettings.GetResistanceStat((RPGSettings.DamageType)(1 << i))] = new Stat();
                stats[RPGSettings.GetDefenceStat((RPGSettings.DamageType)(1 << i))] = new Stat();
            }

            stats[RPGSettings.StatName.Health.ToString()] = new Stat(maxHealth, false);
            healthStat = stats[RPGSettings.StatName.Health.ToString()];

            UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter tpc = GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();
            if (tpc)
                tpc.onGrounded.AddListener(EndKnockback);
        }

        protected void ApplyPassives()
        {
            foreach (Status s in passives)
            {
                // put a permanent boost on the character - 10 million seconds = 115 days
                ApplyStatus(s, 10000000, this as Character, s);
            }
        }

        public virtual void ApplyDamage(float damage, RPGSettings.DamageType dt)
        {
            if (dead)
                return;

            // apply damage resistance
            damage *= GetFactor(-stats[RPGSettings.GetResistanceStat(dt)].currentValue);
            health -= damage;
            if (health <= 0)
            {
                // die!
                if (explosion)
                    explosion.Explode(GetBodyPart(Character.BodyPart.Chest), null);

                dead = true;

                RPGSettings.instance.RemoveCharacter(this);
                foreach (Character ch in Power.getAll())
                    if (ch.target == this)
                        ch.target = null;

                OnDeath();
            }

            int dmg = (int)Mathf.Abs(damage);
            NumberFloat(dmg.ToString(), damage > 0 ? Color.red : Color.green);

            // TODO if we don't have a HUD yet, add a healthbar to props?
            RPGSettings.instance.SetupCharacter(this);
        }

        public void NumberFloat(string msg, Color col)
        {
            NumberFloater numbers = RPGSettings.instance.GetNumberFloater();
            if (numbers)
            {
                numbers.Activate(GetBodyPart(Character.BodyPart.Chest), msg, col);
            }
        }

        IEnumerator DeathFade(Prop p, float time)
        {
            yield return new WaitForSeconds(time);

            p.gameObject.SetActive(false);
        }

        public virtual void OnDeath()
        {
            // TODO shatter props/instantiate a broken version?

            // remove the exiting one
            Destroy(gameObject);
        }

        void OnDead()
        {

        }

        public void UpdateStatus()
        {
            // update out stats if we've lost/gained a status effect
            if (statusDirty)
                ProcessStatus();

            // update the timer on each debuff and remove 
            List<Status> deathRow = new List<Status>();

            foreach (Status status in statusEffects)
            {
                status.timer += Time.deltaTime;
                if (status.isEnded())
                {
                    deathRow.Add(status);
                    statusDirty = true;
                }
                status.UpdateStatus(this);
            }
            foreach (Status d in deathRow)
                statusEffects.Remove(d);
        }

        public float GetFactor(string stat)
        {
            return GetFactor(stats[stat].currentValue);
        }

        public float GetFactor(RPGSettings.StatName stat)
        {
            return GetFactor(stats[stat.ToString()].currentValue);
        }

        public float GetFactor(float val)
        {
            if (val >= 0)
            {
                // buffs - 100 = double, 200 = triple etc. +N%
                return (100.0f + val) / 100.0f;
            }
            else
            {
                // debuffs - -100 = halved, -200 = 1/3 etc
                return (100.0f) / (100.0f - val);
            }
        }

        public Status ApplyStatus(Status s, float duration, Character caster, ScriptableObject power)
        {
            if (s.isImmediate() == false)
            {
                // can we find a status of the same type with the same character and name?
                Status copy = null;
                for (int i = 0; i < statusEffects.Count; i++)
                    if (statusEffects[i].sourceCharacter == caster && statusEffects[i].sourcePower == power && statusEffects[i].name == s.name)
                        copy = statusEffects[i];

                statusDirty = true;
                if (copy)
                {
                    // refresh the duration, and possibly increase how amny stacks we have
                    copy.duration = duration;
                    if (copy.stacks < copy.maxStacks)
                        copy.stacks++;
                    return copy;
                }
                else
                {
                    // status with a duration - add to character's queue
                    Status status = Instantiate(s) as Status;
                    status.stacks = 1;
                    status.name = s.name;
                    status.sourceCharacter = caster;
                    status.sourcePower = power;
                    status.duration = duration; // TODO modify with debuff resistance?
                    statusEffects.Add(status);

                    return status;
                }
            }
            else
            {
                // apply immediately once
                s.Apply(this, caster);
                return null;
            }
        }

        public virtual Transform GetBodyPart(Character.BodyPart part)
        {
            return transform;
        }

        protected virtual void ProcessStatus()
        {
            foreach (KeyValuePair<string, Stat> s in stats)
                if (s.Value.isBuff)
                    s.Value.modifier = 0;

            groupedEffects.Clear();
            hitResponses.Clear();

            // apply all timed buffs sitting on the character
            foreach (Status status in statusEffects)
            {
                // reset all stats to their base values
                status.Apply(this);
                if (groupedEffects.ContainsKey(status.name))
                {
                    groupedEffects[status.name].count+= status.stacks;
                }
                else
                {
                    groupedEffects[status.name] = status;
                    groupedEffects[status.name].count = status.stacks;
                }
            }

            // apply the buffs from a block power directly
            if (activePower && activePower.mode == Power.Mode.Block)
            {
                foreach (Status status in activePower.effects)
                {
                    Buff buff = status as Buff;
                    if (buff)
                    {
                        // reset all stats to their base values
                        status.Apply(this);
                        if (groupedEffects.ContainsKey(status.name))
                        {
                            groupedEffects[status.name].count+=status.stacks;
                        }
                        else
                        {
                            groupedEffects[status.name] = status;
                            groupedEffects[status.name].count = status.stacks;
                        }
                    }
                }

            }

            foreach (KeyValuePair<string, Stat> s in stats)
                s.Value.currentValue = s.Value.getCurrentValue();

            statusDirty = false;
          
            onStatusChanged.Invoke(this, null);
        }

        public static void UpdateProps()
        {
            foreach (GameObject go in activeProps)
            {
                if (go)
                    go.GetComponent<Prop>().UpdateStatus();
            }
        }

        public void ApplyKnockback(Vector3 force)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb)
            {
                // make the character jump
                UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter tpc = GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();
                if (tpc)
                    tpc.forceJump = true;

                // turn off the NavMeshAgent so we can leave the ground
                NavMeshAgent nv = GetComponent<NavMeshAgent>();
                if (nv)
                    nv.enabled = false;

                // TODO - knockback animation

                Character ch = this as Character;
                if (ch)
                    ch.animLock = true;

                // apply a force to them
                rb.velocity = Vector3.zero;
                rb.AddForce(force);
                knockback = true;
            }

        }

        public void EndKnockback()
        {
            // turn Navmesh back on again
            NavMeshAgent nv = GetComponent<NavMeshAgent>();
            if (nv)
                nv.enabled = true;

            // TODO play get up animation


            Character ch = this as Character;
            if (ch)
                ch.animLock = false;

            // apply damage
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb)
            {
                float damage = rb.velocity.magnitude;
                ApplyDamage(damage, RPGSettings.DamageType.Crushing);
                rb.velocity = Vector3.zero;
             }

        }
    }
}
