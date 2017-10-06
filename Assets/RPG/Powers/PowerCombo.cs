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
        public override void OnActivate(Character caster, bool doStatus = true)
        {
            int index = caster.currentComboStage;
            powers[index].OnActivate(caster, doStatus);
        }

        public override Power GetPower(Character caster)
        {
            int index = (caster.currentCombo == this) ? caster.currentComboStage : 0;
            return powers[index];
        }

        public override Power GetStartPower(Character caster)
        {
            int index = caster.currentComboStage;

            //Debug.Log("GetStartPower: index = " + index);

            if (caster.currentCombo != this)
                index = 0;

            caster.currentCombo = this;
            caster.currentComboStage = (index + 1) % powers.Length;
            caster.currentComboTimer = window;

            return powers[index];
        }

        public override float Evaluate(AIBrain brain, AINode.AICondition condition)
        {
            duration = -1.5f;
            Power subP = GetPower(brain.character);
            range = subP.range;
            return subP.Evaluate(brain, condition);
        }

        public override void UpdateAction(AIBrain brain)
        {
            Power subP = GetPower(brain.character);
            brain.target = subP.npcTarget;
            range = subP.range;
            subP.UpdateAction(brain);

            // advance to the next power!
            GetStartPower(brain.character);
        }
    }
}