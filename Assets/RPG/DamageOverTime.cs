using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "DoT", menuName = "RPG/DoT", order = 1)]
    public class DamageOverTime : Status
    {
        public RPGSettings.DamageType damageType;
        public float damagePerTick;
        public float interval;
        public int maxTicks;

        float nextTick = 0;
        int ticks = 0;

        public override void Apply(Prop ch, Character caster = null)
        {

        }

        public override void UpdateStatus(Prop ch)
        {
            if (timer > nextTick)
            {
                if (nextTick > 0)
                {
                    ch.ApplyDamage(damagePerTick, damageType);
                    if (maxTicks != 0)
                    {
                        ticks++;
                        if (ticks >= maxTicks)
                            End();
                    }
                }
                nextTick += interval;
            }
        }

        public override float DamagePerHit()
        {
            // AI eval factor, DoT is worth half of immediate damage
            const float fudgeFactor = 0.5f;

            int numTicks = (int)(duration / interval);
            if (numTicks > maxTicks)
                numTicks = maxTicks;
            return damagePerTick * numTicks * fudgeFactor;
        }

        public override string GetDescription(bool brief = false)
        {
            return name + "\n " + damagePerTick + " " + damageType.ToString() + "damage\n " + "per " + interval + "seconds";
        }
    }
}
