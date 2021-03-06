﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerToggle", menuName = "RPG/Powers/PowerToggle", order = 2)]
    public class PowerToggle : Power
    {
        bool on = false; // move to character
        Status status = null;
        const int longtime = 1000000;

        public class ToggleData
        {
            public bool on;
            public Status[] buffs;
        }

        PowerToggle()
        {
            range = 0;
            type = RPG.RPGSettings.DamageType.Magic;
            targetType = RPG.Power.TargetType.SelfOnly;
            mode = RPG.Power.Mode.Instant;
            tint.code = RPG.RPGSettings.ColorCode.Magic;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerToggle Icon.png");
            }
#endif
        }
        public override void OnActivate(Character caster, bool doStatus = true)
        {
            // create a new toggle data when we require one
            ToggleData data;
            if (caster.toggles.ContainsKey(this) == false)
            {
                data = new ToggleData();
                data.on = false;
                data.buffs = new Status[effects.Length];
                caster.toggles[this] = data;
            }
            else
                data = caster.toggles[this];

            data.on = !data.on;
            if (data.on)
                StartToggle(caster);
            else
                EndToggle(caster);
        }

        public void StartToggle(Character caster)
        {
            ToggleData data = caster.toggles[this];

            // start all buffs off with a really long life
            for (int i = 0; i < effects.Length; i++)
                data.buffs[i] = caster.ApplyStatus(effects[i], longtime, caster, this);
        }

        public void EndToggle(Character caster)
        {
            caster.UsePower(this);

            ToggleData data = caster.toggles[this];

            // end all buffs
            for (int i = 0; i < effects.Length; i++)
            {
                data.buffs[i].End();
                data.buffs[i] = null;
            }
        }

        public bool IsOn(Character caster)
        {
            return caster.toggles.ContainsKey(this) && caster.toggles[this].on;
        }

        public override float Evaluate(AIBrain brain)
        {
            Character character = brain.character;
            // start the toggle off if we're not about to run out of energy from it.
            if (!IsOn(character) && character.energy > 20)
                return BenefitPerHit(character);

            return 0.0f;
        }

        public override void UpdateAction(AIBrain brain)
        {
            OnStart(brain.character);
            OnEnd(brain.character);
        }
    }
}
