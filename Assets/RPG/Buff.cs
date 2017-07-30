using UnityEngine;
using System.Collections;

namespace RPG
{
    [CreateAssetMenu(fileName = "Buff", menuName = "RPG/Buff", order = 1)]
    public class Buff : Status
    {
        public float modifier;
        public Character.StatName stat = Character.StatName.Res;
        public Attack.DamageType damageType;
         
        public override void Apply(Character ch)
        {
            // all stats up to Res require an additional damage type modifier which may be composite
            // all stats after are single use and used as-is
            int dt = (stat <= Character.StatName.Res) ? (int)damageType : 0;

            if (dt != 0) // composite buff eg Physical Resistance, All Resistance
            {
                for (int i = 0; i <= 7; i++)
                {
                    int subType = 1 << i;
                    if ((dt & subType) != 0)
                    {
                        string sn = ((Attack.DamageType)subType).ToString() + stat.ToString();
                        ch.stats[sn].addModifier(modifier);
                    }
                }
            }
            else // other buffs with single stat references
            {
                Stat st = ch.stats[stat.ToString()];
                if (st == null)
                    End();
                st.addModifier(modifier);
            }
        }
    }
}