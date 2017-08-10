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
        public Vector3 rootPos;
        public Vector3 headPos;

        public float maxHealth = 100;
        public float maxEnergy = 100;

        Stat healthStat;
        Stat energyStat;

        const float baseEnergyRegen = 100.0f / 20.0f; // 10 seconds to recover all energy
        const float baseHealthRegen = 100.0f / 120.0f; // 2 minute to heal back to full health
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
        public float energy
        {
            get
            {
                //Some other code
                return energyStat.baseValue;
            }
            set
            {
                //Some other code
                energyStat.baseValue = energyStat.currentValue = Mathf.Clamp(value, 0, maxEnergy);
            }
        }
        public Sprite portrait;
        public string characterName;
        public int team = 2;
        public Power[] powers;
        [HideInInspector]
        public Power activePower;

        Animator animator;

        // used for charges and maintains
        [HideInInspector]
        public float timer;
        [HideInInspector]
        public float nextTick;

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

            for (int i=0; i<RPGSettings.BasicDamageTypesCount; i++)
            {
                stats[RPGSettings.GetResistanceStat((RPGSettings.DamageType)(1 << i))] = new Stat();
                stats[RPGSettings.GetDamageStat((RPGSettings.DamageType)(1 << i))] = new Stat();
            }

            for (RPGSettings.StatName i = RPGSettings.StatName.EnergyRegen; i <= RPGSettings.StatName.Stun; i++)
            {
                float value = 0;
                bool isBuff = true;
                switch (i)
                {
                    case RPGSettings.StatName.Health:
                        value = maxHealth;
                        isBuff = false;
                        break;
                    case RPGSettings.StatName.Energy:
                        value = maxEnergy;
                        isBuff = false;
                        break;
                    case RPGSettings.StatName.Charge:
                        isBuff = false;
                        break;
                }
            
                stats[i.ToString()] = new Stat(value, isBuff);
            }

            healthStat = stats[RPGSettings.StatName.Health.ToString()];
            energyStat = stats[RPGSettings.StatName.Energy.ToString()];

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
            if (beamChild == null)
            {
                GameObject go = Instantiate(RPGSettings.instance.beam);
                beamChild = go.transform;
                beamChild.parent = transform;
                beamChild.localPosition = Vector3.zero;
            }
            if (beamChild)
            {
                beam = beamChild.GetComponent<BeamRenderer>();
            }

            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            // update dictionary for dynamic power swapping
            for (int i = 0; i < powers.Length; i++)
            {
                powerIndexes[powers[i]] = i;
            }

            // resize coolDowns array while preserving dat if we add a power at runtime
            if (coolDowns.Length < powers.Length)
            {
                float[] cds = new float[powers.Length];
                for (int i = 0; i < coolDowns.Length; i++)
                    cds[i] = coolDowns[i];
                coolDowns = cds;
            }

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

            // update health and energy over time
            energy += baseEnergyRegen * GetFactor(RPGSettings.StatName.EnergyRegen) * Time.deltaTime;
            if (energy > maxEnergy)
                energy = maxEnergy;
            health += baseHealthRegen * GetFactor(RPGSettings.StatName.HealthRegen) * Time.deltaTime;
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

            rootPos = GetBodyPart(BodyPart.Root).position;
            headPos = GetBodyPart(BodyPart.Head).position;
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
            coolDowns[powerIndexes[p]] = p.coolDown;
        }

        void ProcessStatus()
        {
            foreach (KeyValuePair<string,Stat> s in stats)
                if (s.Value.isBuff)
                    s.Value.modifier = 0;

            groupedEffects.Clear();

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

            if (tpc)
            {
                tpc.m_JumpPower = baseJumpPower * GetFactor(RPGSettings.StatName.Jump);
                tpc.m_MoveSpeedMultiplier = GetFactor(RPGSettings.StatName.Speed);

                // being stunned allows you to move at half speed
                if (stats[RPGSettings.StatName.Stun.ToString()].currentValue > 0)
                    tpc.m_MoveSpeedMultiplier *= 0.2f;
                if (isRooted())
                    tpc.m_MoveSpeedMultiplier = 0;

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
            damage *= GetFactor(-stats[RPGSettings.GetResistanceStat(dt)].currentValue);

            health -= damage;

            NumberFloater numbers = RPGSettings.instance.GetNumberFloater();
            if (numbers)
            {
                numbers.Activate(GetBodyPart(BodyPart.Head), damage);
            }

            if (health < 0)
            {
                // die!
            }
        }

        public void ApplyStatus(Status s, float duration)
        {
            if (s.isImmediate() == false)
            {
                // status with a duration - add to chracter's queue
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

        // stun and hold stop you from using powers
        public bool isHeld()
        {
            return stats[RPGSettings.StatName.Hold.ToString()].getCurrentValue() > 0 || stats[RPGSettings.StatName.Stun.ToString()].getCurrentValue() > 0;
        }

        // holds and roots prevent the character from moving
        public bool isRooted()
        {
            return stats[RPGSettings.StatName.Hold.ToString()].getCurrentValue() > 0 || stats[RPGSettings.StatName.Root.ToString()].getCurrentValue() > 0;
        }

        public void FaceTarget()
        {
            transform.LookAt(target.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        // Animation functions
        public void PlayAnim(string name)
        {
            animator.Play(name);
            ReleaseAnim(false);
        }

        public void ReleaseAnim(bool release)
        {
            animator.SetBool("release", release);
        }
    }
}
