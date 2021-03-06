﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "Aura", menuName = "RPG/Status/Aura", order = 2)]
    public class Aura : Status
    {
        const float interval = 1;

        float nextTick = 0;
        int ticks = 0;

        public float radius;
        public bool allies = true;

        public Status buff;
        Dictionary<Character, Status> affected = new Dictionary<Character, Status>();

        public override void Apply(Prop ch, Character caster = null)
        {

        }

        // is this right?
        public override float DamagePerHit() { return buff.DamagePerHit(); }
        public override float StatusPerHit(Character target) { return buff.StatusPerHit(target); }
        public override float BenefitPerHit(Character target) { return buff.BenefitPerHit(target); }

        public override void UpdateStatus(Prop p)
        {
            Character caster = p as Character;
            if (!caster)
                return;

            if (timer > nextTick)
            {
                nextTick += interval;

                foreach (Character ch in Power.getAll())
                {
                    if (ch != caster)
                    {
                        if ((ch.team == caster.GetTeam()) == allies)
                        {
                            float dist = Vector3.Distance(ch.transform.position, caster.transform.position);
                            if (dist < radius)
                            {
                                // if we're within the aura, keep pumping up the timer
                                if (affected.ContainsKey(ch) && affected[ch] != null)
                                {
                                    affected[ch].timer = 0;
                                    //Debug.Log("maintain");
                                }
                                else
                                {
                                    affected[ch] = ch.ApplyStatus(buff, 1.5f, caster, this);
                                    //Debug.Log("start");
                                }
                            }
                            else
                            {
                                // out of range, kill the effect and clear the dictionary entry
                                if (affected.ContainsKey(ch) && affected[ch] != null)
                                {
                                    affected[ch].timer = 1.5f;
                                    affected.Remove(ch);
                                    //Debug.Log("remove");
                                }
                            }
                        }
                    }
                }
            }
        }
        public override string GetDescription(bool brief = false)
        {
            return name + "(Aura)" + "\n";
        }
    }
}
