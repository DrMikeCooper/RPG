using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class UIPowerSetTabs : MonoBehaviour
    {
        public PowerSet[] powerSets;
        public PowerSet currentPowerSet;
        public PowersHUD hud;

        void Start()
        {
            // create a button for each set
            PowerSetButton button = GetComponentInChildren<PowerSetButton>();
            GameObject buttonObj = button.gameObject;
            for (int i = 0; i < powerSets.Length; i++)
            {
                GameObject go = (i == 0) ? buttonObj : Instantiate(buttonObj, buttonObj.transform.position + Vector3.right * 48 * i, Quaternion.identity, transform);
                button = go.GetComponent<PowerSetButton>();
                button.SetPowerSet(powerSets[i]);
                button.parent = this;
            }

            currentPowerSet = powerSets[0];
        }

        public void Apply()
        {
            hud.ApplyPowerSet(currentPowerSet);
        }
    }
}