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
        AIAction action;

        public Button button;
        public SquadController controller;
        Image image;

        // Use this for initialization
        void Start()
        {
            button.onClick.AddListener(OnClick);
            image = GetComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Radial360;
        }

        void Update()
        {
            if (image)
            {
                image.fillAmount = 0.5f;
                Power p = action as Power;
                if (p && p.coolDown > 0)
                {

                    image.fillAmount = caster.GetCoolDownFactor(p);
                    //image.color = Color.white * caster.GetCoolDownFactor(p);
                }
            }
        }

        // Set up.
        public void Init(Character cas, Prop targ, AIAction ac)
        {
            caster = cas;
            target = targ;
            action = ac;
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
        }

        // Called when the button is clicked
        void OnClick()
        {
            caster.target = target;
            AIBrain brain = caster.GetComponent<AIBrain>();
            brain.SetRootNode(action.MakeInstance()); // TODO  - make sure Character has a copy
            Power power = brain.rootNode as Power;
            if (power)
                power.npcTarget = target as Character;

            controller.HideActionMenu();
        }
    }
}
