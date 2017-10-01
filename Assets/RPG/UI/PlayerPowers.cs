using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class PlayerPowers : MonoBehaviour, IMenuItemResponder
    {
        Character character;
        TargetPreview preview;
        public Power pendingPower;
        public Power queuedPower;
        Power keyDownPower;
        bool[] keyDown;
        bool[] keyUp;

        // Use this for initialization
        void Start()
        {
            character = GetComponent<Character>();
            preview = GetComponent<TargetPreview>();
            keyDown = new bool[character.powers.Length];
            keyUp = new bool[character.powers.Length];
        }

        // Update is called once per frame
        void Update()
        {
            KeyCode key = KeyCode.Alpha1;
            int index = 0;
            foreach (Power p in character.powers)
            {
                if (Input.GetKeyDown(key) || keyDown[index])
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
                
                if (Input.GetKeyUp(key) || (!keyDown[index] && keyUp[index]))
                {
                    Debug.Log("Actual key up");
                    OnKeyUp(p);
                    keyUp[index] = false;
                }

                keyDown[index] = false;
                key++;
                index++;
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


        public void OnButtonDown(MenuItem item)
        {
            keyDown[item.index] = true;
        }

        public void OnButtonUp(MenuItem item)
        {
            keyUp[item.index] = true;
        }

    }
}