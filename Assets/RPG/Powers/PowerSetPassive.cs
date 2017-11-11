using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerSetPassive", menuName = "RPG/Power Sets/PowerSetPassive", order = 1)]
    public class PowerSetPassive : Power
    {
        public override void OnActivate(Character caster, bool doStatus = true)
        {
            throw new NotImplementedException();
        }
    }
}
