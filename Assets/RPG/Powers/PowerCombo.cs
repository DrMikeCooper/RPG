using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerCombo", menuName = "RPG/PowerCombo", order = 2)]
    public class PowerCombo : Power
    {
        public float window = 3;
        public Power[] powers;
        public override void OnActivate(Character caster)
        {
            int index = caster.currentComboStage;
            powers[index].OnActivate(caster);
        }

        public override Power GetPower(Character caster)
        {
            int index = (caster.currentCombo == this) ? caster.currentComboStage : 0;
            return powers[index];
        }

        public override Power GetStartPower(Character caster)
        {
            int index = caster.currentComboStage;
            if (caster.currentCombo != this)
                index = 0;

            caster.currentCombo = this;
            caster.currentComboStage = (index + 1) % powers.Length;
            caster.currentComboTimer = window;

            return powers[index];
        }
    }
}