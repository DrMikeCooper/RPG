using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerBeam", menuName = "RPG/Powers/PowerBeam", order = 1)]
    public class PowerBeam : PowerDirect
    {
        PowerBeam()
        {
            range = 20;
            type = RPG.RPGSettings.DamageType.Energy;
            targetType = RPG.Power.TargetType.Enemies;
            mode = RPG.Power.Mode.Instant;
            tint.code = RPG.RPGSettings.ColorCode.Energy;
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (icon == null)
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerBeam Icon.png");
                if (beamMaterial == null)
                    beamMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RadialBeam.mat");
            }
#endif
        }
    }
}
