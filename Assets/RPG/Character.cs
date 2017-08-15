using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

namespace RPG
{
    // class representing a character, with health, and status effects applied to them
    public class Character : Prop
    {
        public float maxEnergy = 100;
        Stat energyStat;

        const float baseEnergyRegen = 100.0f / 20.0f; // 10 seconds to recover all energy
        const float baseHealthRegen = 100.0f / 120.0f; // 2 minute to heal back to full health
        
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

        public int team = 2;
        public Power[] powers;

        Animator animator;

        // used for charges and maintains
        [HideInInspector]
        public float timer;
        [HideInInspector]
        public float nextTick;

        AIBrain brain;

        // used by the tab targetter
        [HideInInspector]
        public float xScreen;

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

        Dictionary<Power, float> coolDowns = new Dictionary<Power, float>();
        List<Power> coolDownPowers = new List<Power>();
                    
        ThirdPersonCharacter tpc;
        float baseJumpPower;

        // Use this for initialization
        void Start()
        {
            InitProp();

            fadeTime = 10.0f;

            for (int i=0; i<RPGSettings.BasicDamageTypesCount; i++)
            {
                stats[RPGSettings.GetDamageStat((RPGSettings.DamageType)(1 << i))] = new Stat();
            }

            // special starting values and non-buffs go here
            stats[RPGSettings.StatName.Energy.ToString()] = new Stat(maxEnergy, false);
            stats[RPGSettings.StatName.Charge.ToString()] = new Stat(0, false);

            // all others are normal buffable stats
            for (RPGSettings.StatName i = RPGSettings.StatName.EnergyRegen; i <= RPGSettings.StatName.Stun; i++)
            {
                if (!stats.ContainsKey(i.ToString()))
                    stats[i.ToString()] = new Stat();
            }

            energyStat = stats[RPGSettings.StatName.Energy.ToString()];

            RPGSettings.instance.SetupCharacter(this);

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
            brain = GetComponent<AIBrain>();
            ApplyPassives();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateStatus();

            // update health and energy over time
            energy += baseEnergyRegen * GetFactor(RPGSettings.StatName.EnergyRegen) * Time.deltaTime;
            if (energy > maxEnergy)
                energy = maxEnergy;
            health += baseHealthRegen * GetFactor(RPGSettings.StatName.HealthRegen) * Time.deltaTime;
            if (health > maxHealth)
                health = maxHealth;

            // update cooldowns
            float dt = Time.deltaTime * GetFactor(RPGSettings.StatName.Recharge);
            foreach (Power p in coolDownPowers)
            {
                if (coolDowns[p] > 0)
                {
                    coolDowns[p] -= dt;
                    if (coolDowns[p] < 0)
                        coolDowns[p] = 0;
                }
            }
        }

        // returns 1 for fully recharged, 0 for just used
        public float GetCoolDownFactor(Power p)
        {
            if (p.coolDown == 0)
                return 1.0f;

            return 1.0f - (GetCoolDown(p) / p.coolDown);
        }

        public float GetCoolDown(Power p)
        {
            if (p.coolDown == 0)
                return 0.0f;

            if (!coolDowns.ContainsKey(p))
            {
                coolDowns[p] = 0;
                coolDownPowers.Add(p);
            }
            return coolDowns[p];
        }

        public void UsePower(Power p)
        {
            coolDowns[p] = p.coolDown;
        }

        protected override void ProcessStatus()
        {
            base.ProcessStatus();

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
        }

        


        public override void ApplyDamage(float damage, RPGSettings.DamageType dt)
        {
            // factor in resistance and subtract damage
            base.ApplyDamage(damage, dt);

            // interrupt any ongoing maintains/charges with damage
            if (activePower && activePower.interruptable)
            {
                activePower.OnEnd(this, true);
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

        public override Transform GetBodyPart(BodyPart part)
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

        public void MakeAwareOf(Character caster)
        {
            if (brain && caster)
                brain.MakeAwareOf(caster);
        }

        public bool CanSee(Prop target)
        {
            Vector3 headPos = transform.position;
            headPos.y = chest.position.y;
            Vector3 targetHeadPos = target.transform.position;
            targetHeadPos.y = target.GetBodyPart(BodyPart.Chest).position.y;

            float dist = (targetHeadPos - headPos).magnitude;
            Ray ray = new Ray(headPos, (targetHeadPos - headPos)/dist);
            RaycastHit[] hits = Physics.RaycastAll(ray, dist+1.0f);
            GameObject first = null;
            float bestDistance = dist + 2.0f;
            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != gameObject && hit.distance < bestDistance)
                {
                    bestDistance = hit.distance;
                    first = hit.collider.gameObject;
                }
            }
            return first == target.gameObject;
        }
    }
}
