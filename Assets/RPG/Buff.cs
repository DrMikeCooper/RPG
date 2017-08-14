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
         
        public override void Apply(Prop ch)
        {
            foreach (Modifier mod in modifiers)
            {
                // all stats up to Res require an additional damage type modifier which may be composite
                // all stats after are single use and used as-is
                int dt = (mod.stat <= RPGSettings.StatName.Res) ? (int)damageType : 0;

                if (dt != 0) // composite buff eg Physical Resistance, All Resistance
                {
                    for (int i = 0; i <= 7; i++)
                    {
                        int subType = 1 << i;
                        if ((dt & subType) != 0)
                        {
                            string sn = ((RPGSettings.DamageType)subType).ToString() + mod.stat.ToString();
                            if (ch.stats.ContainsKey(sn))
                                ch.stats[sn].addModifier(mod.modifier);
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
                        st.addModifier(mod.modifier);
                    }
                }
            }
        }
    }
}