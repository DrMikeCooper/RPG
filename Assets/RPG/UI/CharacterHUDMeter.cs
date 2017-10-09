using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{


    public class CharacterHUDMeter : MonoBehaviour
    {
        CharacterHUD hud;

        public RPGSettings.StatName stat;
        public float maximum;
        public Image.FillMethod fillMethod = Image.FillMethod.Horizontal;

        Image image;
        
        // Use this for initialization
        void Start()
        {
            // find the Character HUD parent object
            Transform parent = transform.parent;
            do
            {
                hud = parent.GetComponent<CharacterHUD>();
                parent = parent.parent;
            }
            while (hud == null && parent != null);

            if (hud == null)
                Debug.Log("CharacterHUDMeter " + gameObject.name + "should sit in a CharacterHUD");

            image = GetComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillMethod = fillMethod;

        }

        // Update is called once per frame
        void Update()
        {
            if (hud.character)
            {
                if (stat == RPGSettings.StatName.Health)
                    maximum = hud.character.maxHealth;
                if (stat == RPGSettings.StatName.Energy)
                {
                    Character c = hud.character as Character;
                    if (c)
                        maximum = c.maxEnergy;
                    gameObject.SetActive(c != null);
                }


                float pct = hud.character.stats.ContainsKey(stat.ToString()) ? hud.character.stats[stat.ToString()].currentValue / maximum :  0;
                image.fillAmount = pct;
            }
        }
    }
}