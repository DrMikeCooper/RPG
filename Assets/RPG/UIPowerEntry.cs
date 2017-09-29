using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class UIPowerEntry : MonoBehaviour
    {

        public Text title;
        public Text description;
        public Image icon;
        Power p;
        public Power power
        {
            get { return p; }
            set { p = value; SetPower(); }
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void SetPower()
        {
            if (p != null)
            {
                if (title) title.text = p.name;
                if (description) description.text = p.GetDescription(true);
                if (icon)
                {
                    icon.sprite = p.icon;
                    icon.color = p.tint.GetColor();
                }
            }
            else
            {
                if (title) title.text = "";
                if (description) description.text = "";
                if (icon) icon.sprite = null;
            }

        }
    }
}
