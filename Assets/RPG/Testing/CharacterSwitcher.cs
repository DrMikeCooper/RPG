using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class CharacterSwitcher : MonoBehaviour
    {
        public Character[] characters;
        public int index;
        public KeyCode switchKey;

        public CameraRotate cam;
        public CharacterHUD hud;
        public PowersHUD powerHUD;

        void Start()
        {
            SetCharacter(characters[0]);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(switchKey))
            {
                index = (index + 1) % characters.Length;
                SetCharacter(characters[index]);
            }
        }

        void SetCharacter(Character ch)
        {
            cam.target = ch.transform;
            hud.SetCharacter(ch);
            powerHUD.SetCharacter(ch);

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].GetComponent<PlayerPowers>().enabled = (i == index);
                characters[i].GetComponent<ThirdPersonUserControl>().enabled = (i == index);
            }
        }
    }
}
