using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerMelee", menuName = "RPG/PowerMelee", order = 2)]
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
