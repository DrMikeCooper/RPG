using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

namespace RPG
{
    // class representing a character, with health, and status effects applied to them
    public class Character : MonoBehaviour
    {
        public float maxHealth = 100;
        public float maxEnergy = 100;
        public float health;
        public float energy;
        public Sprite portrait;
        public string characterName;
        public int team = 2;
        public Power[] powers;

        public enum BodyPart
        {
            Root,
            Head,
            Chest,
            LeftHand,
            RightHand,
            LeftFoot,
            RightFoot
        }

        public Transform head;
        public Transform chest;
        public Transform leftHand;
        public Transform rightHand;
        public Transform leftFoot;
        public Transform rightFoot;

        [HideInInspector]
        public BeamRenderer beam;

        [HideInInspector]
        public float[] coolDowns;
        Dictionary<Power, int> powerIndexes = new Dictionary<Power, int>();

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

        [HideInInspector]
        public Character target; // who is this character targetting (can be null or yourself)?

        ThirdPersonCharacter tpc;
        float baseJumpPower;

        // Use this for initialization
        void Start()
        {
            statusDirty = true;
            stats = new Dictionary<string, Stat>();
            statusEffects = new List<Status>();
            groupedEffects = new Dictionary<string, Status>();

            for (int i=0; i<8; i++)
            {
                stats[RPGSettings.GetResistanceStat((RPGSettings.DamageType)(1 << i))] = new Stat();
                stats[RPGSettings.GetDamageStat((RPGSettings.DamageType)(1 << i))] = new Stat();
            }

            // health and energy regeneration multipliers
            stats[RPGSettings.StatName.HealthRegen.ToString()] = new Stat();
            stats[RPGSettings.StatName.EnergyRegen.ToString()] = new Stat(5);
            stats[RPGSettings.StatName.Jump.ToString()] = new Stat();
            stats[RPGSettings.StatName.Speed.ToString()] = new Stat();
            stats[RPGSettings.StatName.Recharge.ToString()] = new Stat();

            health = maxHealth;
            energy = maxEnergy;

            RPGSettings.instance.SetupCharacter(this);

            // time remaining till each power can be used
            coolDowns = new float[powers.Length];

            for (int i = 0; i < powers.Length; i++)
            {
                powerIndexes[powers[i]] = i;
            }

            tpc = GetComponent<ThirdPersonCharacter>();
            if (tpc)
                baseJumpPower = tpc.m_JumpPower;

            Transform beamChild = transform.Find("Beam");
            if (beamChild)
            {
                beam = beamChild.GetComponent<BeamRenderer>();
            }
        }

        // Update is called once per frame
        void Update()
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
            }
            foreach (Status d in deathRow)
                statusEffects.Remove(d);

            // update health and energy over time
            energy += stats[RPGSettings.StatName.EnergyRegen.ToString()].currentValue * Time.deltaTime;
            if (energy > maxEnergy)
                energy = maxEnergy;
            health += stats[RPGSettings.StatName.HealthRegen.ToString()].currentValue * Time.deltaTime;
            if (health > maxHealth)
                health = maxHealth;

            // update cooldowns
            for (int i = 0; i < coolDowns.Length; i++)
            {
                if (coolDowns[i] > 0)
                {
                    coolDowns[i] -= Time.deltaTime * GetFactor(RPGSettings.StatName.Recharge);
                    if (coolDowns[i] < 0)
                        coolDowns[i] = 0;
                }
            }
        }

        // returns 1 for fully recharged, 0 for just used
        public float GetCoolDownFactor(int i)
        {
            return 1.0f - (coolDowns[i] / powers[i].coolDown);
        }

        public float GetCoolDown(Power p)
        {
            return coolDowns[powerIndexes[p]];
        }

        public void UsePower(Power p)
        {
            energy -= p.energyCost;
            coolDowns[powerIndexes[p]] = p.coolDown;
        }

        void ProcessStatus()
        {
            foreach (KeyValuePair<string,Stat> s in stats)
                s.Value.modifier = 0;

            groupedEffects.Clear();

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

            foreach (KeyValuePair<string, Stat> s in stats)
                s.Value.currentValue = s.Value.getCurrentValue();

            statusDirty = false;

            if (tpc)
            {
                tpc.m_JumpPower = baseJumpPower * GetFactor(RPGSettings.StatName.Jump);
                tpc.m_MoveSpeedMultiplier = GetFactor(RPGSettings.StatName.Speed);

            }
            onStatusChanged.Invoke();
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


        public void ApplyDamage(float damage, RPGSettings.DamageType dt)
        {
            // apply damage resistance
            damage *= 100.0f / (100.0f + stats[RPGSettings.GetResistanceStat(dt)].currentValue);

            health -= damage;
            if (health < 0)
            {
                // die!
            }
        }

        public void ApplyStatus(Status s, float duration)
        {
            Status status = Instantiate(s) as Status;
            status.duration = duration; // TODO modify with debuff resistance?
            statusEffects.Add(status);
            statusDirty = true;
        }

        public void UseEnergy(float e)
        {
            energy -= e;
            if (energy < 0)
            {
                energy = 0;
                // TODO?
            }
        }

        public float GetHealthPct() { return health / maxHealth; }
        public float GetEnergyPct() { return energy / maxEnergy; }

        public Transform GetBodyPart(BodyPart part)
        {
            switch (part)
            {
                case BodyPart.Head: return head; 
                case BodyPart.Chest: return chest; 
                case BodyPart.LeftHand: return leftHand;
                case BodyPart.RightHand: return rightHand;
                case BodyPart.LeftFoot: return leftFoot;
                case BodyPart.RightFoot: return rightFoot;
                default: return transform;
            }
        }
    }
}
