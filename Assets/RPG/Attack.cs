using UnityEngine;
using System.Collections;

namespace RPG
{
    // base class for all attacks
    public class Attack : MonoBehaviour
    {
        public enum DamageType
        {
            Crushing,
            Piercing,
            Fire,
            Cold,
            Energy,
            Negative,
            Psionic,
            Magic
        };

        public static string GetResistanceStat(DamageType dt) { return dt.ToString() + "Res"; }
        public static string GetDamageStat(DamageType dt) { return dt.ToString() + "Dam"; }

        public float damageMin, damageMax;
        public DamageType damageType;
        public Status[] effects;
        public float energyCost;
        public float coolDown;
        public float timer;

        public void Apply(Character source, Character target)
        {
            float damage = Random.Range(damageMin, damageMax);

            damage *= (100.0f + source.stats[Attack.GetResistanceStat(damageType)].currentValue)*0.01f;
            target.ApplyDamage(damage, damageType);

            // apply any status effects 
            foreach(Status s in effects)
            {
                target.ApplyStatus(s);
            }
        }

    }
}