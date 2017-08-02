﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "PowerProjectile", menuName = "RPG/PowerProjectile", order = 1)]
    public class PowerProjectile : Power
    {
        public float speed = 1;
        public ParticleSystem projectileParticles;
        public override void OnActivate(Character caster)
        {
            // check standard cooldown, energy cost etc
            if (!CanUse(caster))
                return;

            // energy and cooldowns
            UsePower(caster);

            // spawn a projectile and set it going in the right direction
            GameObject go = Instantiate(RPGSettings.instance.projectile);
            go.transform.position = caster.GetBodyPart(userBodyPart).position;
            Projectile proj = go.GetComponent<Projectile>();
            Vector3 velocity = caster.transform.forward * speed;
            proj.Init(this, caster, velocity);

            // create particles on the projectile
            GameObject pgo = Instantiate(projectileParticles.gameObject);
            pgo.transform.parent = go.transform;
            pgo.transform.localPosition = Vector3.zero;


        }
    }
}
