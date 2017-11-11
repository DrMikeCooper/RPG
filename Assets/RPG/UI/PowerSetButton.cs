using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class PowerSetButton : MonoBehaviour {

        public PowerSet powerSet;
        public UIPowerList list;

        void Start()
        {
            Image image = GetComponent<Image>();
            if (image)
            {
                image.sprite = powerSet.icon;
                image.color = powerSet.tint.GetColor();
            }
        }

        public void OnClick()
        {
            list.SetPowerSet(powerSet);
        }
    }
}