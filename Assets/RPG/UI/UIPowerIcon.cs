using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class UIPowerIcon : IDraggable
    {
        
        public Power power;

        void Start()
        {
            Init();
            SetPower(power);
        }

        public void SetPower(Power p)
        {
            power = p;
        }

        public Power GetPower()
        {
            return power;
        }
    }
}
