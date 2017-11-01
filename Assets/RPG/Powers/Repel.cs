using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// put this in Prop.ApplyStatus
//                if (returnVal as Repel)
//                  returnVal.Apply(this, caster);
namespace RPG
{
    [CreateAssetMenu(fileName = "Repel", menuName = "RPG/Status/Repel", order = 2)]
    public class Repel : Status
    {
        [HideInInspector]
        public Vector3 direction;
        public float velocity = 1.0f;

        public override void Apply(Prop ch, Character caster = null)
        {
            if (caster)
                direction = (ch.transform.position - caster.transform.position).normalized;
        }

        public override void UpdateStatus(Prop ch)
        {
            ch.transform.position += Time.deltaTime * velocity * direction;
        }

        public override string GetDescription(bool brief = false)
        {
            return "Repel";
        }
    }
}
