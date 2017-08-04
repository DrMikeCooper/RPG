using UnityEngine;
using System.Collections;

namespace RPG
{
    public abstract class Status : ScriptableObject
    {
        [HideInInspector]
        public float duration;
        public ParticleSystem particles;
        public Character.BodyPart bodyPart = Character.BodyPart.Root;
        public Sprite icon;
        public RPGSettings.ColorCode color;

        [HideInInspector]
        public float timer = 0;
        [HideInInspector]
        public int count = 0;

        public void End()
        {
            timer = duration;
        }

        public bool isEnded()
        {
            return timer >= duration;
        }

        public abstract void Apply(Character ch);
        public virtual bool isImmediate() { return false; } // true if the status applies instantly
    }
}
