using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class UIPowerList : MonoBehaviour
    {
        public GameObject prefab;
        public PowerSet powerSet;

        void Start()
        {
            SetPowerSet(powerSet);
        }

        // Use this for initialization
        public void SetPowerSet(PowerSet ps)
        {
            powerSet = ps;
            ScrollRect sr = GetComponent<ScrollRect>();
            Transform par = sr ? sr.content : transform;

            // clean up old list of children
            for (int i = 0; i < par.childCount; i++)
            {
                Destroy(par.GetChild(i).gameObject);
            }

            int y = 0;
            foreach (Power p in powerSet.powers)
            {
                GameObject go = ObjectFactory.GetObject(prefab);
                UIPowerEntry pe = go.GetComponent<UIPowerEntry>();
                pe.power = p;
                go.transform.parent = par;
                go.transform.localPosition = new Vector3(100, -16+y, 0);
                y -= 33;
            }
            if (sr)
            {
                sr.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                sr.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 33 * powerSet.powers.Length);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
