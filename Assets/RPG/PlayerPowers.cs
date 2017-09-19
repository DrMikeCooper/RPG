using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class PlayerPowers : MonoBehaviour
    {
        Character character;
        TargetPreview preview;

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            preview = GetComponent<TargetPreview>();
        }

        // Update is called once per frame
        void Update()
        {
            KeyCode key = KeyCode.Alpha1;
            foreach (Power p in character.powers)
            {
                if (Input.GetKeyDown(key) && character.activePower == null && character.animLock == false)
                {
                    p.OnStart(character);
                    if (preview)
                        preview.StartPreview(p);
                }
                
                if (Input.GetKeyUp(key))
                {
                    if (character.activePower != null) // == p.GetPower(character))
                        p.OnEnd(character);
                    if (preview) preview.EndPreview();
                }
                key++;
            }

            if (character.activePower)
            {
                character.activePower.OnUpdate(character);
            }
        }

        
    }
}