using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class PlayerPowers : MonoBehaviour, IMenuItemResponder
    {
        Character character;
        TargetPreview preview;

        // the power that's been held down but can't activate yet due to range, energy or cooldown
        public Power pendingPowerStart;
        public Power pendingPowerEnd;

        // queued power while another one has been activated, which will trigger once the activePower is clear
        public int queuedPower = -1;

        // array of inputs that have happened this frame
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
            // read the keyboard
            for (int i=0; i< character.powers.Length; i++)
            {
                KeyCode key = KeyCode.Alpha1 + i;
                if (Input.GetKeyDown(key)) keyDown[i] = true;
                if (Input.GetKeyUp(key)) keyUp[i] = true;
            }

            // check each power for activation
            for (int i = 0; i < character.powers.Length; i++)
            {
                Power p = character.powers[i];
                if (keyDown[i])
                {
                    if (character.activePower == null && character.animLock == false)
                    {
                        // queue the power up for starting
                        if (preview)
                            preview.StartPreview(p);

                        pendingPowerStart = p;
                        pendingPowerEnd = null;
                    }
                    keyDown[i] = false;
                }
                
                if (keyUp[i])
                {
                    pendingPowerEnd = p;
                    keyUp[i] = false;
                }
            }

            // once the powers queued, start when cooldown and energy and everything else permit
            if (pendingPowerStart && pendingPowerStart.GetPower(character).CanUse(character))
            {
                pendingPowerStart.OnStart(character);
                pendingPowerStart = null;
            }

            if (pendingPowerEnd)
            {
                pendingPowerEnd.OnEnd(character);

                if (character.activePower != null) 
                    pendingPowerEnd.OnEnd(character);
                if (preview) preview.EndPreview();

                pendingPowerEnd = null;
            }

            if (character.activePower)
            {
                character.activePower.OnUpdate(character);
            }
        }

        // simulated keypresses using buttons on screen
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