using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerDirect", menuName = "RPG/Powers/PowerDirect", order = 1)]
    public class PowerDirect : Power
    {
        protected PowerDirect()
        {
            range = 20;
            type = RPG.RPGSettings.DamageType.Magic;
            targetType = RPG.Power.TargetType.Enemies;
            mode = RPG.Power.Mode.Instant;
            tint.code = RPG.RPGSettings.ColorCode.Magic;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerDirect Icon.png");
            }
#endif
        }

        // useable on a single target with no to-hit roll
        public override void OnActivate(Character caster, bool doStatus = true)
        {
            Prop target = GetTarget(caster);

            if (target)
            {
                float charge = caster.stats[RPGSettings.StatName.Charge.ToString()].currentValue * 0.01f;
                Character ctarget = target as Character;
                if (ctarget)
                    ctarget.MakeAwareOf(caster);

                // no deflection, carry on...
                bool hit = Apply(target, charge, caster, doStatus);
            }
        }

        public override float Evaluate(AIBrain brain, AINode.AICondition condition)
        {
            return EvaluateRanged(brain, condition);
        }

        public override void UpdateAction(AIBrain brain)
        {
            OnUpdateRanged(brain);
        }
    }
}