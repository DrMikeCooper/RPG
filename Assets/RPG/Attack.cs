using UnityEngine;
using System.Collections;

namespace RPG
{
    // base class for all attacks
    public class Attack : MonoBehaviour
    {
        public enum DamageType
        {
            Crushing = 1,
            Piercing = 2,
            Fire = 4,
            Cold = 8,
            Energy = 16,
            Negative = 32,
            Psionic = 64,
            Magic = 128,
            Physical = Crushing + Piercing,
            Elemental = Fire + Cold + Energy,
            Paranormal = Negative + Psionic + Magic,
            All = 255
        };

        public static bool IsComposite(DamageType dt)
        {
            return (dt == DamageType.All) || (dt == DamageType.Physical) || (dt == DamageType.Paranormal) || (dt == DamageType.Elemental);
        }

        // if an attack gets set a composite type, return an appropriate damage type for it.
        static DamageType GetDamageType(DamageType dt)
        {
            if (dt == DamageType.All || dt == DamageType.Physical)
                return DamageType.Crushing;
            if (dt == DamageType.Elemental)
                return DamageType.Fire;
            if (dt == DamageType.Paranormal)
                return DamageType.Magic;
            return dt;
        }

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