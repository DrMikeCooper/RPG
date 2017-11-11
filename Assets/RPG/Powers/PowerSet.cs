using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerSet", menuName = "RPG/Power Sets/PowerSet", order = 1)]
    public class PowerSet : ScriptableObject
    {
        public Sprite icon;
        public RPGSettings.Tint tint;
        public Power[] powers;

        public void ApplyToCharacter(Character ch)
        {

        }
    }
}
