using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class OverheadHUD : MonoBehaviour
    {
        Transform target;
        RectTransform rect;

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
        }
    }
}