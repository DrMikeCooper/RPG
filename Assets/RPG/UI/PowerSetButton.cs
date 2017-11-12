using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class PowerSetButton : MonoBehaviour {

        public UIPowerSetTabs parent;
        public PowerSet powerSet;
        public UIPowerList list;

        public void SetPowerSet(PowerSet ps)
        {
            powerSet = ps;
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
            parent.currentPowerSet = powerSet;
        }
    }
}