using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "Arc", menuName = "RPG/Status/Arc", order = 2)]
    public class Arc : Status
    {
        [Header("Arc Settings")]
        public PowerBeam beam;

        public override void Apply(Prop ch, Character caster = null)
        {
            Character ctarget = ch as Character;

            // doens't work with props right now
            if (ctarget == null)
                return;

            List<Character> targets = new List<Character>();
            Character[] all = PowerArea.getAll();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != ch && (ctarget==null || all[i].GetTeam() == ctarget.team))
                    targets.Add(all[i]);
            }

            beam.Arc(ctarget, targets, 1);
        }

        public override string GetDescription(bool brief = false)
        {
            return "Arc " + beam.name;
        }

        public override bool isImmediate() { return true; }

        public override float DamagePerHit()
        {
            return beam.DamagePerHit();
        }
    }
}
