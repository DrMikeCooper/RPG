using UnityEngine;
using System.Collections;
using System;

namespace RPG
{
    [CreateAssetMenu(fileName = "Buff", menuName = "RPG/Buff", order = 1)]
    public class Buff : Status
    {
        [Serializable]
        public struct Modifier
        {
            public float modifier;
            public RPGSettings.StatName stat;
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
                if (mod.stat == RPGSettings.StatName.Hold || mod.stat == RPGSettings.StatName.Stun)
                    value = Mathf.Max(value, !target.isHeld() ? 0 : 70);
                else if (mod.stat == RPGSettings.StatName.Root)
                    value = Mathf.Max(value, !target.isRooted() ? 0 : 30);
                else if (mod.stat <= RPGSettings.StatName.Def) // buffs to basic combat abilities stack - 100% modifier to damage etc is bonus of 50
                    value = Mathf.Max(value, Mathf.Abs(mod.modifier) * 0.5f);
                else // run speed, energy regen etc, nice to have
                    value = Mathf.Max(value, Mathf.Abs(mod.modifier) * 0.25f);
            }
            return value;
        }
    }
}