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
        
        public override void Apply(Character ch)
        {
        }

        public override void UpdateStatus(Character ch)
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
    }
}
