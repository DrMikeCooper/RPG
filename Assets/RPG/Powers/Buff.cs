using UnityEngine;
using System.Collections;
using System;

namespace RPG
{
    [CreateAssetMenu(fileName = "Buff", menuName = "RPG/Status/Buff", order = 1)]
    public class Buff : Status
    {
        [Serializable]
        public struct Modifier
        {
            public float modifier;
            public RPGSettings.StatName stat;

            public string GetDescription(RPGSettings.DamageType damageType)
            {
                string desc = (modifier > 0 ? "+" : "") + modifier + (RPGSettings.IsMez(stat) ? " " : "% to ");
                if (stat <= RPGSettings.StatName.Def) desc += damageType.ToString() +" ";
                desc += stat.ToString();
                return desc;
            }
        }
        public Modifier[] modifiers;

        [Tooltip("Used only for Defence and Resistance debuffs")]
        public RPGSettings.DamageType damageType;

        public override void Apply(Prop ch, Character caster = null)
        {
            bool awakenProp = false;

            foreach (Modifier mod in modifiers)
            {
                // all stats up to Res require an additional damage type modifier which may be composite
                // all stats after are single use and used as-is
                int dt = (mod.stat <= RPGSettings.StatName.Def) ? (int)damageType : 0;

                if (dt != 0) // composite buff eg Physical Resistance, All Resistance
                {
                    for (int i = 0; i <= 8; i++)
                    {
                        int subType = 1 << i;
                        if ((dt & subType) != 0)
                        {
                            string sn = ((RPGSettings.DamageType)subType).ToString() + mod.stat.ToString();
                            if (ch.stats.ContainsKey(sn))
                            {
                                awakenProp = true;
                                ch.stats[sn].addModifier(mod.modifier * stacks);
                            }
                        }
                    }
                }
                else // other buffs with single stat references
                {
                    string sn = mod.stat.ToString();
                    if (ch.stats.ContainsKey(sn))
                    {
                        Stat st = ch.stats[sn];
                        if (st == null)
                            End();
                        else
                        {
                            awakenProp = true;
                            st.addModifier(mod.modifier * stacks);
                        }
                    }
                }
            }
            if (awakenProp && (ch as Character) == null)
            {
                if (Prop.activeProps.Contains(ch.gameObject) == false)
                    Prop.activeProps.Add(ch.gameObject);
            }

        }
        public override float StatusPerHit(Character target)
        {
            // some kind of heuristic to equate debuffs and holds to damage
            // 100 hp is considered to be the ultimate debuff of death ie full disablement for ever!

            float value = 0;
            foreach (Modifier mod in modifiers)
            {
                if (mod.stat == RPGSettings.StatName.Hold || mod.stat == RPGSettings.StatName.Stun)
                    value = Mathf.Max(value, target.isHeld() ? 0 : 70);
                else if (mod.stat == RPGSettings.StatName.Root)
                    value = Mathf.Max(value, target.isRooted() ? 0 : 30);
                else if (mod.stat <= RPGSettings.StatName.Def) // buffs to basic combat abilities stack - 100% modifier to damage etc is bonus of 50
                    value = Mathf.Max(value, Mathf.Abs(-mod.modifier) * 0.5f);
                else // run speed, energy regen etc, nice to inflict but not crippling
                    value = Mathf.Max(value, Mathf.Abs(-mod.modifier) * 0.25f);
            }
            return value;
        }

        public override float BenefitPerHit(Character target)
        {
            // heuristic to determine how useful a buff is

            float value = 0;
            foreach (Modifier mod in modifiers)
            {
                float value0 = 0;
                if (mod.stat == RPGSettings.StatName.Hold || mod.stat == RPGSettings.StatName.Stun)
                    value0 = !target.isHeld() ? 0 : 70;
                else if (mod.stat == RPGSettings.StatName.Root)
                    value0 =  !target.isRooted() ? 0 : 30;
                else if (mod.stat <= RPGSettings.StatName.Def) // buffs to basic combat abilities stack - 100% modifier to damage etc is bonus of 50
                    value0 = Mathf.Abs(mod.modifier) * 0.5f;
                else // run speed, energy regen etc, nice to have
                    value0 = Mathf.Abs(mod.modifier) * 0.25f;

                //how many stacks of this buff does the character have?
                int numStacks = target.GetStacks(this);
                if (numStacks > 0)
                    value0 *= 0.5f;
                if (numStacks == maxStacks)
                    value0 = 0;

                value = Mathf.Max(value, value0);
            }
            return value;
        }

        public override string GetDescription(bool brief = false)
        {
            if (modifiers.Length == 0)
                return name;

            string desc = "";
            if (brief)
            {
                for (int k =0; k<modifiers.Length; k++)
                    desc += modifiers[k].GetDescription(damageType) + ((k != modifiers.Length - 1) ? ", " : "");
            }
            else
            {
                desc = name + "\n ";
                foreach (Modifier mod in modifiers)
                    desc += mod.GetDescription(damageType) + "\n ";
            }
            return desc;
        }

        public override bool IsCurrentlyActive(Prop p)
        {
            foreach (Modifier mod in modifiers)
            {
                if (RPGSettings.IsMez(mod.stat))
                    if (p.stats.ContainsKey(mod.stat.ToString()))
                    {
                        float val = p.stats[mod.stat.ToString()].getCurrentValue();
                        Debug.Log("ActiveCheck: " + mod.stat.ToString() + " = " + val);
                        if (val <= 0)
                            return false;
                    }
            }
            return true;
        }
    }
}