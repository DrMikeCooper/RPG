using UnityEngine;
using System.Collections;

namespace RPG
{
    public class Stat
    {
        public Stat() { }
        public Stat(float val) { baseValue = val; }

        // base value derived from the character
        public float baseValue = 0;
        // current modifier on the stat from buffs or debuffs
        public float modifier = 0;
        // the current calculated value based on the modifier
        public float currentValue = 0;

        public void addModifier(float delta)
        {
            modifier += delta;
        }

        public float getCurrentValue()
        {
            return baseValue + modifier;

            /*if (modifier > 0)
                return baseValue * (1 + modifier);
            else
                return baseValue * (1 - modifier);*/
        }
    }
}
