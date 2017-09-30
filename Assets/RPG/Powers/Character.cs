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
        [HideInInspector]
        public bool powerStarted = false;

        Animator animator;

        // used for charges and maintains
        [HideInInspector]
        public float timer;
        [HideInInspector]
        public float nextTick;

        //[HideInInspector]
        public float animCountDown;

        // combo information
        public PowerCombo currentCombo = null;
        public int currentComboStage = 0;
        public float currentComboTimer = 0;

        // temp data for toggle powers
        public Dictionary<PowerToggle, PowerToggle.ToggleData> toggles = new Dictionary<PowerToggle, PowerToggle.ToggleData>();

        AIBrain brain;
        GameObject reticle;

        public bool animLock
        {
            get
            {
                return animCountDown > 0;
            }
        }

        // used by the tab targetter
        [HideInInspector]
        public float xScreen;
        [HideInInspector]
        public AudioSource audioSource;

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

        Dictionary<string, float> coolDowns = new Dictionary<string, float>();
        List<string> coolDownPowers = new List<string>();
                    
        ThirdPersonCharacter tpc;
        float baseJumpPower;

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            brain = GetComponent<AIBrain>();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            InitProp();

            fadeTime = 10.0f;

            for (int i=0; i<RPGSettings.BasicDamageTypesCount; i++)
            {
                stats[RPGSettings.GetDamageStat((RPGSettings.DamageType)(1 << i))] = new Stat();
            }
            // healing damage boost does make sense
            stats[RPGSettings.GetDamageStat(RPGSettings.DamageType.Healing)] = new Stat();

            // special starting values and non-buffs go here
            stats[RPGSettings.StatName.Energy.ToString()] = new Stat(maxEnergy, false);
            stats[RPGSettings.StatName.Charge.ToString()] = new Stat(0, false);

            // all others are normal buffable stats
            RPGSettings.StatName numStats = (RPGSettings.StatName)System.Enum.GetNames(typeof(RPGSettings.StatName)).Length;
            for (RPGSettings.StatName i = RPGSettings.StatName.EnergyRegen; i <= numStats; i++)
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

            ApplyPassives();

            //create a tragetting reticle and disable it
            reticle = Instantiate(RPGSettings.instance.reticle);
            reticle.transform.parent = transform;
            reticle.transform.localPosition = Vector3.zero;
            reticle.name = "reticle";
            reticle.SetActive(false);
        }

        public void ResetCombos()
        {
            currentCombo = null;
            currentComboStage = 0;
            currentComboTimer = 0;
        }

        // Update is called once per frame
        void Update()
        {
            // count down animation lock
            if (animCountDown > 0)
                animCountDown -= Time.deltaTime;

            // count down combo windows
            if (currentComboTimer > 0)
            {
                currentComboTimer -= Time.deltaTime;
                if (currentComboTimer <= 0)
                {
                    ResetCombos();
                }
            }

            foreach (KeyValuePair<PowerToggle, PowerToggle.ToggleData> pair in toggles)
            {
                PowerToggle power = pair.Key;
                PowerToggle.ToggleData data = pair.Value;
                if (data.on)
                {
                    energy -= power.extraEnergyCost * Time.deltaTime;
                    if (energy == 0)
                    {
                        data.on = false;
                        power.EndToggle(this);
                    }
                }
            }
            
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
            foreach (string p in coolDownPowers)
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

            if (!coolDowns.ContainsKey(p.name))
            {
                coolDowns[p.name] = 0;
                coolDownPowers.Add(p.name);
            }
            return coolDowns[p.name];
        }

        public void UsePower(Power p)
        {
            coolDowns[p.name] = p.coolDown;
        }

        protected override void ProcessStatus()
        {
            base.ProcessStatus();
            if (brain)
            {
                brain.ResetEnemies();
                brain.UpdateEnemies();
            }

            if (tpc)
            {
                tpc.m_JumpPower = baseJumpPower * GetFactor(RPGSettings.StatName.Jump);
                tpc.m_MoveSpeedMultiplier = GetFactor(RPGSettings.StatName.Speed);

                // being stunned allows you to move at half speed
                float maxSpeed = 1;
                if (stats[RPGSettings.StatName.Stun.ToString()].currentValue > 0)
                    maxSpeed = 0.2f;
                if (isRooted())
                    maxSpeed = 0;
                tpc.maxSpeed = maxSpeed;
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
            powerStarted = false;
        }

        // animation event response
        public void Hit()
        {
            // for instant powers, trigger it now
            if (activePower && activePower.IsInstant())
            {
                // trigger it
                activePower.OnActivate(this);
                activePower = null;
            }
            // flag the power as ready to go for charge, maintain etc
            powerStarted = true;
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

        void ProcessAnimations()
        {
            string[] names = AnimationUtilities.GetEnumNames<Power.Animations>();
            AnimationUtilities.ProcessAnimations(animator, names, (AnimationClip a)=> AnimationUtilities.AddEventAtEnd(a, "EndPowerAnim"));
        }

        // death things
        IEnumerator DeathFade(Prop p, float time)
        {
            yield return new WaitForSeconds(time);

            p.gameObject.SetActive(false);
        }

        public override void OnDeath()
        {
            PlayAnim("Die");
            StartCoroutine(DeathFade(this, 10));
        }

        public void AddAIDebugText(Character caster, string msg)
        {
            AIBrain brain = caster.GetComponent<AIBrain>();
            if (brain && brain.showDebug)
            {
                OverheadHUD hud = RPGSettings.instance.GetHUD(this);
                if (brain.debugText != null)
                    hud.debugText = brain.debugText;
                hud.AddDebugText(msg);
            }
        }

        public GameObject GetReticle() { return reticle; }
        public void ShowReticle(Color color)
        {
            reticle.SetActive(true);
            reticle.GetComponent<MeshRenderer>().material.SetColor("_TintColor", color);
        }

        public int GetTeam()
        {
            if (stats[RPGSettings.StatName.Confusion.ToString()].getCurrentValue() > 0)
                team = 3-team; // swap between 1 and 2 for now.
            if (stats[RPGSettings.StatName.Enrage.ToString()].getCurrentValue() > 0)
                team = -1;
            return team;
        }
    }
}

