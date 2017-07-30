using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace RPG
{
    // class representing a character, with health, and status effects applied to them
    public class Character : MonoBehaviour
    {
        public float maxHealth = 100;
        public float maxEnergy = 100;
        public float health;
        public float energy;

        public List<Status> statusEffects;
        public Dictionary<string, Status> groupedEffects;

        public bool statusDirty;
        public UnityEvent onStatusChanged = new UnityEvent();

        public Dictionary<string, Stat> stats;

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

            health = maxHealth;
            energy = maxEnergy;

            RPGSettings.instance.SetupCharacter(this);
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

            onStatusChanged.Invoke();
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

        public void ApplyStatus(Status s)
        {
            Status status = Instantiate(s) as Status;
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
    }
}
