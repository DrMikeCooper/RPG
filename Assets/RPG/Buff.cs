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

        public Buff() { }
        //Buff(string n, string sn, float f, float d, Color c, Attack.DamageType dt = 0) { name = n; statName = sn; modifier = f; duration = d;  color = c; damageType = (int)dt; }
        //public Buff(Buff b) { modifier = b.modifier; name = b.name; statName = b.statName;  duration = b.duration; count = b.count; color = b.color; damageType = b.damageType; }

        public override void Apply(Character ch)
        {
            int dt = (int)damageType;
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