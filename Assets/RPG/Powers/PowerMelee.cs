using System.Collections;
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
        }
    }
}
