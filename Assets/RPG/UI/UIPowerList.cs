using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class UIPowerList : MonoBehaviour
    {
        public GameObject prefab;
        public Power[] powers;

        // Use this for initialization
        void Start()
        {
            ScrollRect sr = GetComponent<ScrollRect>();
            Transform par = sr ? sr.content : transform;

            int y = 0;
            foreach (Power p in powers)
            {
                GameObject go = ObjectFactory.GetObject(prefab);
                UIPowerEntry pe = go.GetComponent<UIPowerEntry>();
                pe.power = p;
                go.transform.parent = par;
                go.transform.localPosition = new Vector3(100, -33+y, 0);
                y -= 66;
            }
            if (sr)
            {
                sr.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                sr.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 66 * powers.Length);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
