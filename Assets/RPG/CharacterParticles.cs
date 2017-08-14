﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class CharacterParticles : MonoBehaviour
    {
        Prop character;

        Dictionary<string, GameObject> systems = new Dictionary<string, GameObject>();

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Prop>();
            character.onStatusChanged.AddListener(UpdateParticles);
        }

        void UpdateParticles()
        {
            foreach (KeyValuePair<string,GameObject> pair in systems)
            {
                if (character.groupedEffects.ContainsKey(pair.Key) == false)
                {
                    // turn on the fader to deactivate it
                    LifeSpanFader fader = pair.Value.GetComponent<LifeSpanFader>();
                    if (!fader)
                        Debug.Log("Missing fader!");
                    if (fader && !fader.enabled)
                    {
                        fader.lifespan = 1;
                        fader.enabled = true;
                    }
                }
            }

            foreach (KeyValuePair<string, Status> pair in character.groupedEffects)
            {
                // if we dont have a particle system for this key yet, make one
                if (systems.ContainsKey(pair.Key) == false)
                {
                    Status s = pair.Value;
                    if (s.fx)
                        systems[pair.Key] = s.fx.Begin(character.GetBodyPart(s.bodyPart), s.color, false);
                }
                else
                {
                    LifeSpanFader fader = systems[pair.Key].GetComponent<LifeSpanFader>();
                    if (!fader)
                        Debug.Log("Missing fader!");
                    else
                        fader.Restart();
                }
            }
        }
    }
}
