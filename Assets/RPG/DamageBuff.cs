using UnityEngine;
using System.Collections;
using System;

namespace RPG
{
    public class DamageBuff : Status
    {
        public DamageBuff() { }
        public DamageBuff(DamageBuff db) { buff = db.buff; duration = db.duration; }
        public float buff;

        public override void Apply(Character ch)
        {
            
        }

        public override Status Clone()
        {
            return new DamageBuff(this);
        }
    }
}
