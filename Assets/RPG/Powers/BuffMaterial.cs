using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "BuffMaterial", menuName = "RPG/BuffMaterial", order = 1)]
    public class BuffMaterial : Status
    {
        // these match the enumeration RPGSettings.DamageTypes
        public float crushingRes = 0;
        public float piercingRes = 0;
        public float toxicRes = 0;
        public float fireRes = 0;
        public float coldRes = 0;
        public float energyRes = 0;
        public float electricRes = 0;
        public float magicRes = 0;
        public float psionicRes = 0;
        public float negativeRes = 0;

        public override void Apply(Prop ch, Character caster = null)
        {
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Crushing)].addModifier(crushingRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Piercing)].addModifier(piercingRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Toxic)].addModifier(toxicRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Fire)].addModifier(fireRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Cold)].addModifier(coldRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Energy)].addModifier(energyRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Electric)].addModifier(electricRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Psionic)].addModifier(psionicRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Magic)].addModifier(magicRes);
            ch.stats[RPGSettings.GetResistanceStat(RPGSettings.DamageType.Negative)].addModifier(negativeRes);
        }

        public override float StatusPerHit(Character target) { return -BenefitPerHit(target); }
        public override float BenefitPerHit(Character target) { return (crushingRes + piercingRes + toxicRes + fireRes + coldRes + energyRes + electricRes + magicRes + psionicRes + negativeRes) *0.1f; }

        public override string GetDescription(bool brief = false)
        {
            string desc = name + "(Material)\n";
            if (!brief)
            {
                string list = "";

            }

            return desc;
        }
    }
}
