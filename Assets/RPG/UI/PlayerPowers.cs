using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class PlayerPowers : MonoBehaviour
    {
        Character character;
        TargetPreview preview;
        Power pendingPower;

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
                    // queue the power up for starting
                    pendingPower = p;
                    if (preview)
                        preview.StartPreview(p);
                }
                
                if (Input.GetKeyUp(key))
                {
                    pendingPower = null;
                    if (character.activePower != null) // == p.GetPower(character))
                        p.OnEnd(character);
                    if (preview) preview.EndPreview();
                }
                key++;
            }

            // once the powers queued, start when cooldown and energy permit
            if (pendingPower && pendingPower.CanUse(character))
                pendingPower.OnStart(character);

            if (character.activePower)
            {
                character.activePower.OnUpdate(character);
            }
        }

        
    }
}