using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    // makes the caster perform the action against their target
    public class MenuItem : MonoBehaviour
    {
        Character caster;
        Prop target;
        [HideInInspector]
        public AIAction action;

        public Button button;
        public SquadController controller;
        Image image;
        IMenuItemResponder responder;
        public int index;

        // Use this for initialization
        void Start()
        {
            button.onClick.AddListener(OnClick);
            image = GetComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillAmount = 1;
            image.fillMethod = Image.FillMethod.Radial360;
        }

        void Update()
        {
            if (image)
            {
                Power p = action as Power;
                if (p && p.coolDown > 0)
                {
                    image.fillAmount = caster.GetCoolDownFactor(p);
                }
                else
                    image.fillAmount = 1.0f;
                if (p && button.image)
                {
                    Color color = p.CanUse(caster) ? p.tint.GetColor() : p.tint.GetColor() * 0.5f;
                    color.a = 1;
                    button.image.color = color;
                } 
            }
        }

        // Set up.
        public void Init(Character cas, Prop targ, AIAction ac, int ind)
        {
            caster = cas;
            target = targ;
            action = ac;
            index = ind;
            Power power = action as Power;
            if (power)
            {
                button.image.sprite = power.icon;

                ToolTipIcon toolTip = GetComponent<ToolTipIcon>();
                if (toolTip)
                {
                    toolTip.OnPointerExit();
                    Text text = toolTip.toolTip.GetComponentInChildren<Text>();
                    if (text)
                        text.text = power.GetDescription();
                }
            }

            // could be AIBrain (squad based setup) or PlayerPower (third person setup)
            responder = caster.GetComponent<IMenuItemResponder>();
        }

        public void SetPower(Power p)
        {
            action = p;
            button.image.sprite = p.icon;
        }

        public void SetTarget(Prop t)
        {
            target = t;
        }

        void OnButtonDown()
        {
            responder.OnButtonDown(this);
        }

        void OnButtonUp()
        {
            OnClick();
        }

        // Called when the button is clicked
        void OnClick()
        {
            caster.target = target;
            responder.OnButtonDown(this);
            responder.OnButtonUp(this);

            if (controller)
                controller.HideActionMenu();
        }
    }
}
