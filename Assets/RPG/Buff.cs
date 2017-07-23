using UnityEngine;
using System.Collections;

namespace RPG
{
    public class Buff : Status
    {
        public float modifier;
        Stat stat = null;


        public Buff() { }
        Buff(string n, float f, float d, Color c) { name = n;  modifier = f; duration = d;  color = c; }
        public Buff(Buff b) { modifier = b.modifier; name = b.name; duration = b.duration; count = b.count; color = b.color; }

        public override void Apply(Character ch)
        {
            if (stat == null)
            {
                stat = ch.stats[name];
                if (stat == null)
                    End();
            }
            stat.addModifier(modifier);
        }

        public override Status Clone()
        {
            return new Buff(this);
        }

        public static Buff GetResBuff(Attack.DamageType dt, float f, float d)
        {
            return new Buff(Attack.GetResistanceStat(dt), f, d, Color.cyan);
        }

        public static Buff GetRegenBuff(float f, float d)
        {
            return new Buff(Character.HealthRegen, f, d, Color.red);
        }

        public static Buff GetEnergiseBuff(float f, float d)
        {
            return new Buff(Character.EnergyRegen, f, d, Color.blue);
        }
    }
}