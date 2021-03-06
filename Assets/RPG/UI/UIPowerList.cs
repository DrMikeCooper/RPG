﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class UIPowerList : MonoBehaviour, IDragAndDropContainer
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
                UIPowerIcon icon = go.GetComponentInChildren<UIPowerIcon>();
                if (icon)
                    icon.SetPower(p);
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

        public bool CanDrag(IDraggable obj)
        {
            return true;
        }

        public bool CanDrop(IDraggable dragged, IDraggable drop)
        {
            // can never drop into this type of container
            return true;
        }

        public void Drop(IDraggable dragged, IDraggable drop, int replacedIndex, bool final)
        {
            // do nothing
        }

        public bool DoesSwap()
        {
            return false;
        }
    }
}
