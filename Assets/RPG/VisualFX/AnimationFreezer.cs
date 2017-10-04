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
            animator = transform.parent.GetComponent<Animator>();
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