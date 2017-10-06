using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class ChildSorter : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            List<OverheadHUD> huds = new List<OverheadHUD>();
            OverheadHUD[] hudArray = transform.GetComponentsInChildren<OverheadHUD>();
            foreach (OverheadHUD hud in hudArray)
                huds.Add(hud);

            huds.Sort(delegate (OverheadHUD a, OverheadHUD b) {
                return a.screenPos.z.CompareTo(b.screenPos.z);
            });

            foreach (OverheadHUD hud in huds)
                hud.transform.SetAsFirstSibling();
        }
    }
}
