using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class AttackAnimation : StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Character ch = animator.gameObject.GetComponent<Character>();
            if (ch)
                ch.animLock = false;
        }
    }
}
