using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class DebugHUD : MonoBehaviour {

        Text text;
        public Character ch;

        void Start()
        {
            text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update() {
            string txt = "";
            foreach (KeyValuePair<string, Stat> pair in ch.stats)
            {
                Stat s = pair.Value;
                txt += pair.Key + ":" + s.currentValue + "\n";
            }
            text.text = txt;
        }
    }
}
