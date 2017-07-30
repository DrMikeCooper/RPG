using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "AttackData", menuName = "RPG/AttackData", order = 1)]
    public class AttackData : ScriptableObject {

        public float energyCost;
        public float range;
        Attack.DamageType damageType;
    }
}
