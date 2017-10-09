using UnityEngine;
using System.Collections;

namespace RPG
{
    public abstract class Status : ScriptableObject
    {
        [HideInInspector]
        public float duration;
        [Header("Visual FX")]
        public VisualEffect fx;
        public Character.BodyPart bodyPart = Character.BodyPart.Root;
        public Sprite icon;
        public RPGSettings.Tint tint;
        [Header("Stacking Rules")]
        public int maxStacks = 0;
        [ShowIf("maxStacks", ShowIfAttribute.Comparison.Not, 0)]
        [Tooltip("Untick this if you want this buff to stack from all sources. Tick it and each character/power source will maintain a different stack")]
        public bool sourceSelectiveStacks = true;
        [HideInInspector]
        public int stacks = 0;
        [HideInInspector]
        public Character sourceCharacter;
        [HideInInspector]
        public ScriptableObject sourcePower; // could be a Power or HitResponse

        [HideInInspector]
        public float timer = 0;
        [HideInInspector]
        public int count = 0;

        // the condition to check against the caster before using
        [Tooltip("Condition that must be met to apply this status")]
        public AINode.AICondition condition;

        public void End()
        {
            timer = duration;
        }

        public bool isEnded()
        {
            return timer >= duration;
        }

        public abstract void Apply(Prop ch, Character caster = null);
        public abstract string GetDescription(bool brief = false);
        public virtual bool isImmediate() { return false; } // true if the status applies instantly
        public virtual void UpdateStatus(Prop ch) {}

        public virtual float DamagePerHit() { return 0; }
        public virtual float StatusPerHit(Character target) { return 0; }
        public virtual float BenefitPerHit(Character target) { return 0; }

        // return false if the status is dormant eg a Hold or Stun that's being neutralised by resistance to such effects
        public virtual bool IsCurrentlyActive(Prop p) { return true; }
    }
}
