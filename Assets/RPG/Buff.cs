using UnityEngine;
using System.Collections;

namespace RPG
{
    public class Buff : Status
    {
        public float modifier;
        Stat stat = null;
        string statName;

        public Buff() { }
        Buff(string n, string sn, float f, float d, Color c) { name = n; statName = sn; modifier = f; duration = d;  color = c; }
        public Buff(Buff b) { modifier = b.modifier; name = b.name; statName = b.statName;  duration = b.duration; count = b.count; color = b.color; }

        public override void Apply(Character ch)
        {
            if (stat == null)
            {
                stat = ch.stats[statName];
                if (stat == null)
                    End();
            }
            stat.addModifier(modifier);
        }

        public override Status Clone()
        {
            return new Buff(this);
        }

        public static Buff GetResBuff(Attack.DamageType dt, float f, float d, string name = "")
        {
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