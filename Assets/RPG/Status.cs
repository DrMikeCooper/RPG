using UnityEngine;
using System.Collections;

namespace RPG
{
    public abstract class Status : ScriptableObject
    {
        [HideInInspector]
        public float duration;
        //public ParticleSystem particles;
        public VisualEffect fx;
        public Character.BodyPart bodyPart = Character.BodyPart.Root;
        public Sprite icon;
        public RPGSettings.ColorCode color;
        public int maxStacks = 1;
        public int stacks = 0;
        public Character sourceCharacter;
        public ScriptableObject sourcePower; // could be a Power or HitResponse

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

        public abstract void Apply(Prop ch);
        public virtual bool isImmediate() { return false; } // true if the status applies instantly
        public virtual void UpdateStatus(Prop ch) {}

        public virtual float DamagePerHit() { return 0; }
        public virtual float StatusPerHit(Character target) { return 0; }
        public virtual float BenefitPerHit(Character target) { return 0; }

    }
}
