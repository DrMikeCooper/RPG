using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
        public PowerArea explosion;

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
        public List<Status> statusEffects;
        [HideInInspector]
        public Dictionary<string, Status> groupedEffects;

        [HideInInspector]
        public bool statusDirty;
        [HideInInspector]
        public UnityEvent onStatusChanged = new UnityEvent();

        [HideInInspector]
        public Dictionary<string, Stat> stats;

        // here in the base class for logic in ProcessStatus, only ever non-null on characters
        [HideInInspector]
        public Power activePower;

        public List<HitResponse> hitResponses = new List<HitResponse>();

        protected float fadeTime = 0.5f;

        // global list of atcive props to receive updates for DoTs etc
        public static List<GameObject> activeProps = new List<GameObject>();
        
        // Use this for initialization
        void Start()
        {
            InitProp();
        }

        protected void InitProp()
        {
            statusDirty = true;
            stats = new Dictionary<string, Stat>();
            statusEffects = new List<Status>();
            groupedEffects = new Dictionary<string, Status>();

            for (int i = 0; i < RPGSettings.BasicDamageTypesCount; i++)
            {
                stats[RPGSettings.GetResistanceStat((RPGSettings.DamageType)(1 << i))] = new Stat();
            }

            stats[RPGSettings.StatName.Health.ToString()] = new Stat(maxHealth, false);
            healthStat = stats[RPGSettings.StatName.Health.ToString()];
        }

        public virtual void ApplyDamage(float damage, RPGSettings.DamageType dt)
        {
            // apply damage resistance
            damage *= GetFactor(-stats[RPGSettings.GetResistanceStat(dt)].currentValue);
            health -= damage;
            if (health <= 0)
            {
                // die!
                if (explosion)
                    explosion.Explode(GetBodyPart(Character.BodyPart.Chest), null);
                Destroy(gameObject, fadeTime);
            }

            NumberFloater numbers = RPGSettings.instance.GetNumberFloater();
            if (numbers)
            {
                numbers.Activate(GetBodyPart(Character.BodyPart.Chest), damage);
            }

            // TODO if we don't have a HUD yet, add a healthbar to props?

        }

        void OnDestroy()
        {
            RPGSettings.instance.RemoveCharacter(this);
            foreach (Character ch in Power.getAll())
                if (ch.target == this)
                    ch.target = null;
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

        public void ApplyStatus(Status s, float duration)
        {
            if (s.isImmediate() == false)
            {
                // status with a duration - add to character's queue
                Status status = Instantiate(s) as Status;
                status.name = s.name;
                status.duration = duration; // TODO modify with debuff resistance?
                statusEffects.Add(status);
                statusDirty = true;
            }
            else
            {
                // apply immediately once
                s.Apply(this);
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
                    groupedEffects[status.name].count++;
                }
                else
                {
                    groupedEffects[status.name] = status;
                    groupedEffects[status.name].count = 1;
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
                            groupedEffects[status.name].count++;
                        }
                        else
                        {
                            groupedEffects[status.name] = status;
                            groupedEffects[status.name].count = 1;
                        }
                    }
                }

            }

            foreach (KeyValuePair<string, Stat> s in stats)
                s.Value.currentValue = s.Value.getCurrentValue();

            statusDirty = false;
          
            onStatusChanged.Invoke();
        }

        public static void UpdateProps()
        {
            foreach (GameObject go in activeProps)
            {
                if (go)
                    go.GetComponent<Prop>().UpdateStatus();
            }
        }
    }
}
