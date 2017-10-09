using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "Repel", menuName = "RPG/Status/Repel", order = 2)]
    public class Repel : Status
    {
        [HideInInspector]
        public Vector3 direction;

        public override void Apply(Prop ch, Character caster = null)
        {
           // TODO - figure out how to make characters move
        }

        public override string GetDescription(bool brief = false)
        {
            return "Repel";
        }
    }
}
