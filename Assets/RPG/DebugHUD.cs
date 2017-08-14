using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class DebugHUD : MonoBehaviour {

        Text text;
        public Prop ch;

        void Start()
        {
            text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.RightShift))
                text.enabled = !text.enabled;

            if (text.enabled)
            {
                Prop ch0 = ch;
                if (ch.target != null)
                    ch = ch0.target;

                string txt = "";
                foreach (KeyValuePair<string, Stat> pair in ch0.stats)
                {
                    Stat s = pair.Value;
                    txt += pair.Key + ":" + s.currentValue + "\n";
                }
                text.text = txt;
            }
            
        }
    }
}
