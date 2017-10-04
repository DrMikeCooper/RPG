using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class AnimationFreezer : MonoBehaviour
    {
        Animator animator;

        // Use this for initialization
        void Start()
        {
            // look up the heirarchy for an animator
            Transform t = transform;
            while (animator == null && t.parent != null)
            {
                t = t.parent;
                animator = t.GetComponent<Animator>();
            }
            TurnOn();
        }

        // Use this for initialization
        void OnEnable()
        {
            TurnOn();
        }

        void OnDisable()
        {
            TurnOff();
        }

        void TurnOn()
        {
            if (animator)
                animator.enabled = false;
        }

        void TurnOff()
        {
            animator.enabled = true;
        }
    }
}