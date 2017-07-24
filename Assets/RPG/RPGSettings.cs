using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class RPGSettings : MonoBehaviour
    {
        public GameObject overheadHUD;
        public static RPGSettings instance;

        // Use this for initialization
        void Awake()
        {
            instance = this;
        }

        public void SetupCharacter(Character character)
        {
            // copy the HUD template and make it this
            GameObject hud = Instantiate(overheadHUD);
            hud.name = "HUD_" + character.name;
            hud.transform.SetParent(transform);
            hud.GetComponent<CharacterHUD>().character = character;
        }
    }
}