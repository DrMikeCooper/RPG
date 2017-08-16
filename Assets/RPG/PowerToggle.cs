using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerToggle", menuName = "RPG/PowerToggle", order = 2)]
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
    
        public override void OnActivate(Character caster)
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
                data.buffs[i] = caster.ApplyStatus(effects[i], longtime);
        }

        public void EndToggle(Character caster)
        {
            ToggleData data = caster.toggles[this];

            // end all buffs
            for (int i = 0; i < effects.Length; i++)
            {
                data.buffs[i].End();
                data.buffs[i] = null;
            }
        }

    }
}
