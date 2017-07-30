using UnityEngine;
using System.Collections;

namespace RPG
{
    public class Buff : Status
    {
        public float modifier;
        string statName;
        int damageType = 0;

        public Buff() { }
        Buff(string n, string sn, float f, float d, Color c, Attack.DamageType dt = 0) { name = n; statName = sn; modifier = f; duration = d;  color = c; damageType = (int)dt; }
        public Buff(Buff b) { modifier = b.modifier; name = b.name; statName = b.statName;  duration = b.duration; count = b.count; color = b.color; damageType = b.damageType; }

        public override void Apply(Character ch)
        {
            if (damageType != 0) // composite buff eg Physical Resistance, All Resistance
            {
                for (int i = 0; i <= 7; i++)
                {
                    int subType = 1 << i;
                    if ((damageType & subType) != 0)
                    {
                        string sn = ((Attack.DamageType)subType).ToString() + statName;
                        ch.stats[sn].addModifier(modifier);
                    }
                }
            }
            else // other buffs with single stat references
            {
                Stat stat = ch.stats[statName];
                if (stat == null)
                    End();
                stat.addModifier(modifier);
            }
        }

        public override Status Clone()
        {
            return new Buff(this);
        }

        public static Buff GetResBuff(Attack.DamageType dt, float f, float d, string name = "")
        {
            if (Attack.IsComposite(dt))
                return new Buff(name, "Res", f, d, Color.cyan, dt);
            return new Buff(name, Attack.GetResistanceStat(dt), f, d, Color.cyan);
        }

        public static Buff GetRegenBuff(float f, float d, string name = "")
        {
            return new Buff(name, Character.HealthRegen, f, d, Color.red);
        }

        public static Buff GetEnergiseBuff(float f, float d, string name = "")
        {
            return new Buff(name, Character.EnergyRegen, f, d, Color.blue);
        }
    }
}