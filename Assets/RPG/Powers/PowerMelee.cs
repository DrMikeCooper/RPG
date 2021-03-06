﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerMelee", menuName = "RPG/Powers/PowerMelee", order = 1)]
    public class PowerMelee : PowerDirect
    {
        PowerMelee()
        {
            range = 1;
            targetType = TargetType.Enemies;
            type = RPGSettings.DamageType.Crushing;
            mode = RPG.Power.Mode.Instant;
            tint.code = RPG.RPGSettings.ColorCode.Crushing;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerMelee Icon.png");
            }
#endif
        }
    }
}
