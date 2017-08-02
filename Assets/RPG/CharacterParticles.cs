using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class CharacterParticles : MonoBehaviour
    {
        Character character;

        Dictionary<string, ParticleSystem> systems = new Dictionary<string, ParticleSystem>();

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            character.onStatusChanged.AddListener(UpdateParticles);
        }

        void UpdateParticles()
        {
            foreach (KeyValuePair<string,ParticleSystem> pair in systems)
            {
                if (character.groupedEffects.ContainsKey(pair.Key) == false)
                {
                    pair.Value.Stop();
                    // todo, delayed turn off after 2 seconds?
                }
            }

            foreach (KeyValuePair<string, Status> pair in character.groupedEffects)
            {
                // if we dont have a particle system for this key yet, make one
                if (systems.ContainsKey(pair.Key) == false)
                {
                    Status s = pair.Value;
                    ParticleSystem particles = s.particles;
                    if (particles)
                    {
                        GameObject obj = Instantiate(particles.gameObject);
                        ParticleSystem system = obj.GetComponent<ParticleSystem>();
                        obj.transform.parent = character.GetBodyPart(s.bodyPart);
                        obj.transform.localPosition = Vector3.zero;
                        obj.name = "Particles_" + pair.Key;
                        system.startColor = RPGSettings.GetColor(s.color); // figure this out...
                        systems[pair.Key] = system;
                    }
                }
                else
                {
                    ParticleSystem system = systems[pair.Key];
                    if (system && system.isStopped)
                        system.Play();
                }
               

                
            }
        }
    }
}
