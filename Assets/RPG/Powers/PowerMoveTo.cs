using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerMoveTo", menuName = "RPG/Powers/PowerMoveTo", order = 2)]
    public class PowerMoveTo : PowerDirect
    {
        PowerMoveTo()
        {
            range = 10;
            type = RPG.RPGSettings.DamageType.Crushing;
            targetType = RPG.Power.TargetType.Enemies;
            mode = RPG.Power.Mode.MoveTo;
            closeToTargetSpeed = 20;
            tint.code = RPG.RPGSettings.ColorCode.Crushing;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerMoveTo Icon.png");
            }
#endif
        }
    }
}
