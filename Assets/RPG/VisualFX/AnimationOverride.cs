using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverride : MonoBehaviour {

    public string anim;

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
        {
            animator.CrossFade(anim, 0.05f);
            animator.SetBool("release", false);
        }
    }

    void TurnOff()
    {
        if (animator)
            animator.SetBool("release", true);
    }
}
