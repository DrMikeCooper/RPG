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
            foreach (AnimatorClipInfo clipInfo in clips)
            {
                AnimationUtilities.AddEventAtEnd(clipInfo.clip, "EndPowerAnim");
            }
        }
    }
}
