using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class CharacterParticles : MonoBehaviour
    {
        Character character;

        Dictionary<string, GameObject> systems = new Dictionary<string, GameObject>();

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            character.onStatusChanged.AddListener(UpdateParticles);
        }

        void UpdateParticles()
        {
            foreach (KeyValuePair<string,GameObject> pair in systems)
            {
                if (character.groupedEffects.ContainsKey(pair.Key) == false)
                {
                    // todo - use end somehow?
                    Destroy(pair.Value);
                }
            }

            foreach (KeyValuePair<string, Status> pair in character.groupedEffects)
            {
                // if we dont have a particle system for this key yet, make one
                if (systems.ContainsKey(pair.Key) == false)
                {
                    Status s = pair.Value;
                    systems[pair.Key] = s.fx.Begin(character.GetBodyPart(s.bodyPart), s.color);
                }
            }
        }
    }
}
