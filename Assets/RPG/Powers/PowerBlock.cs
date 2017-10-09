using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerBlock", menuName = "RPG/Powers/PowerBlock", order = 2)]
    public class PowerBlock : PowerDirect
    {
        PowerBlock()
        {
            range = 0;
            targetType = TargetType.SelfOnly;
            type = RPGSettings.DamageType.Crushing;
            mode = RPG.Power.Mode.Block;
            tint.code = RPG.RPGSettings.ColorCode.Crushing;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerBlock Icon.png");
            }
#endif
        }
    }
}
