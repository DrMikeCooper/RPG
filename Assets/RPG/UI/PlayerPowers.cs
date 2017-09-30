using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class PlayerPowers : MonoBehaviour
    {
        Character character;
        TargetPreview preview;
        public Power pendingPower;
        public Power queuedPower;
        Power keyDownPower;

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
                if (Input.GetKeyDown(key))
                {
                    if (character.activePower == null && character.animLock == false)
                    {
                        // queue the power up for starting
                        pendingPower = p;
                        if (preview)
                            preview.StartPreview(p);
                    }
                    else
                        queuedPower = p;
                    keyDownPower = p;
                }
                
                if (Input.GetKeyUp(key))
                {
                    Debug.Log("Actual key up");
                    OnKeyUp(p);
                }
                key++;
            }

            if (queuedPower != null && pendingPower == null && character.activePower == null && character.animLock == false)
            {
                pendingPower = queuedPower;
                queuedPower = null;
            }

            // once the powers queued, start when cooldown and energy and everything else permit
            if (pendingPower && pendingPower.GetPower(character).CanUse(character))
            {
                pendingPower.OnStart(character);
                Debug.Log("onKeyUp called from pending");
                OnKeyUp(pendingPower);
            }

            if (character.activePower)
            {
                character.activePower.OnUpdate(character);
            }
        }

        void OnKeyUp(Power p)
        {
            pendingPower = null;

            if (p != keyDownPower)
                return;
            keyDownPower = null;

            if (character.activePower != null) // == p.GetPower(character))
                p.OnEnd(character);
            if (preview) preview.EndPreview();
        }
        
    }
}