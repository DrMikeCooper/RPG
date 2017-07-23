using UnityEngine;
using System.Collections;

namespace RPG
{
    public abstract class Status
    {
        public float timer = 0;
        public float duration;
        public string name;
        public int count = 0;
        public Color color;

        public void End()
        {
            timer = duration;
        }

        public bool isEnded()
        {
            return timer >= duration;
        }

        public abstract Status Clone();
        public abstract void Apply(Character ch);
    }
}
