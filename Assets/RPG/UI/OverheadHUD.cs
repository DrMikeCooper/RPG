﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class OverheadHUD : MonoBehaviour
    {
        Transform target;
        RectTransform rect;

        public GameObject debugPanel;
        public Text debugText;

        public Vector3 screenPos;

        // Use this for initialization
        void Start()
        {
            target = GetComponent<CharacterHUD>().character.transform;
            rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 targetPos = target.position + 2.5f * Vector3.up;
            rect.position = targetPos;
            transform.forward = Camera.main.transform.forward;

            screenPos = Camera.main.WorldToScreenPoint(targetPos);

            if (debugPanel && debugText)
                debugPanel.SetActive(debugText.text != "");
        }

        public void ClearDebugText()
        {
            if (debugText)
                debugText.text = "";
        }

        public void AddDebugText(string msg)
        {
            if (debugText)
                debugText.text += (msg + "\n");
        }
    }
}