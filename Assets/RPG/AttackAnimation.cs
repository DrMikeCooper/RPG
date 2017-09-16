using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class AttackAnimation : StateMachineBehaviour
    {
        // When we fire off one of these, make sure we have the right animation events at the end
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(layerIndex);
            if (clips.Length > 0)
                animator.GetComponent<Character>().animCountDown = clips[0].clip.length;
            else
                animator.GetComponent<Character>().animCountDown = 1;

        }
    }
}
