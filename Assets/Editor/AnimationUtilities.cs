using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

namespace RPG
{
    public class AnimationUtilities
    {
        public static void AddEventAtEnd(AnimationClip clip, string name)
        {
            // see if we already have an EndEvent
            bool hasEndEvent = false;
            foreach (AnimationEvent e in clip.events)
                if (e.functionName == name)
                    hasEndEvent = true;

            // if not, stick one on the end
            if (!hasEndEvent)
            {
                AnimationEvent e = new AnimationEvent();
                e.functionName = name;
                e.time = clip.length;
                clip.AddEvent(e);
            }
        }

        public static string[] GetEnumNames<T>()
        {
            // get the number of animation names we use
            System.Array arr = System.Enum.GetValues(typeof(T));
            int num = arr.Length;

            string[] names = new string[num];
            for (int i = 0; i < num; i++)
            {
                names[i] = arr.GetValue(i).ToString();
            }
            return names;
        }

        // find a named node in an animator
        public AnimatorState FindState(Animator animator, string name)
        {
            AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
            for (int i = 0; i < animator.layerCount; i++)
            {
                ChildAnimatorState[] aStates = ac.layers[i].stateMachine.states;
                for (int j = 0; j < aStates.Length; j++)
                {
                    if (aStates[j].state.name == name)
                        return aStates[j].state;
                }
            }

            return null;
        }

        public static void ProcessAnimations(Animator animator, string[] nodeNames, System.Action<AnimationClip> fn)
        {
            // iterate over the animator graph's node, checking if the node is one we'll be triggering as a power animation
            // and stick a 
            AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
            for (int i = 0; i < animator.layerCount; i++)
            {
                ChildAnimatorState[] aStates = ac.layers[i].stateMachine.states;
                for (int j = 0; j < aStates.Length; j++)
                {
                    ChildAnimatorState aState = aStates[j];

                    // check if its a power anim, by comparing the node name with known power anim names
                    bool onList = false;
                    for (int k = 0; k < nodeNames.Length; k++)
                    {
                        if (aState.state.name == nodeNames[k])
                            onList = true;
                    }

                    if (onList)
                    {
                        // see if the node has an animation
                        AnimationClip clip = aState.state.motion as AnimationClip;

                        if (clip)
                        {
                            fn.Invoke(clip);
                        }
                    }
                }
            }
        }

        public static void AddTransitionToState(Animator animator, string[] nodeNames, AnimatorStateTransition transition)
        {
            // iterate over the animator graph's node, checking if the node is one we'll be triggering as a power animation
            // and stick a 
            UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            for (int i = 0; i < animator.layerCount; i++)
            {
                ChildAnimatorState[] aStates = ac.layers[i].stateMachine.states;
                for (int j = 0; j < aStates.Length; j++)
                {
                    UnityEditor.Animations.ChildAnimatorState aState = aStates[j];
                   
                    // check if its a power anim, by comparing the node name with known power anim names
                    bool onList = false;
                    for (int k = 0; k < nodeNames.Length; k++)
                    {
                        if (aState.state.name == nodeNames[k])
                            onList = true;
                    }

                    if (onList)
                    {
                        // check if we already go to the state in the required transition
                        bool alreadySet = false;
                        for (int k = 0; k < aState.state.transitions.Length; k++)
                        {
                            if (aState.state.transitions[k].destinationState == transition.destinationState)
                                alreadySet = true;
                        }

                        // if not, add the required transition
                        if (!alreadySet)
                            aState.state.AddTransition(transition);
                        else
                        {
                            // make sure we've got all the conditions?
                        }
                    }
                }
            }
        }
    }
}
