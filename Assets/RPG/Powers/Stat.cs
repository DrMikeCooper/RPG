using UnityEngine;
using System.Collections;

namespace RPG
{
    public class Stat
    {
        public Stat() { }
        public Stat(float val, bool buff = true) { baseValue = val; currentValue = val; isBuff = buff; }

        // base value derived from the character and their gear
        public float baseValue = 0;
        // current modifier on the stat from buffs or debuffs
        public float modifier = 0;
        // the current calculated value based on the modifier
        public float currentValue = 0;

        public bool isBuff = true;

        public void addModifier(float delta)
        {
            modifier += delta;
        }

        public float getCurrentValue()
        {
            return baseValue + modifier;
        }
    }
}
