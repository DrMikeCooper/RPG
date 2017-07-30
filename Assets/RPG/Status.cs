using UnityEngine;
using System.Collections;

namespace RPG
{
    public abstract class Status : ScriptableObject
    {
        public float duration;
        public ParticleSystem particles;
        public Sprite icon;
        public Color color = Color.white;

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
    }
}
