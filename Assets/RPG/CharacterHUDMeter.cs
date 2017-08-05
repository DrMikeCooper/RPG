using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{


    public class CharacterHUDMeter : MonoBehaviour
    {
        CharacterHUD hud;

        public RPGSettings.StatName stat;
        public float maximum;

        RectTransform meter;
        Rect initialRect;

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

            meter = GetComponent<RectTransform>();
            initialRect = meter.rect;
        }

        // Update is called once per frame
        void Update()
        {
            if (hud.character)
            {
                if (stat == RPGSettings.StatName.Health)
                    maximum = hud.character.maxHealth;
                if (stat == RPGSettings.StatName.Energy)
                    maximum = hud.character.maxEnergy;


                float pct = hud.character.stats[stat.ToString()].currentValue / maximum;
                meter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pct * initialRect.width);
                Vector3 pos = meter.localPosition;
                pos.x = 0.5f * (pct - 1.0f) * initialRect.width;
                meter.localPosition = pos;
            }
        }
    }
}