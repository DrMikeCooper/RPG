using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    [CustomEditor(typeof(Character))]
    public class CharacterDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            Character ch = (Character)target;

            // Show default inspector property editor
            DrawDefaultInspector();

            if (GUILayout.Button("Find Body Parts"))
            {
                FindBodyParts(ch);
            }

            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onDead"), EditorStyles.standardFont);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onDamaged"), EditorStyles.standardFont);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStatusChanged"), EditorStyles.standardFont);
        }

        void FindBodyParts(Character ch)
        {
            ch.head = null;
            ch.chest = null;
            ch.leftHand = null;
            ch.rightHand = null;
            ch.leftFoot = null;
            ch.rightFoot = null;

            Transform[] children = ch.GetComponentsInChildren<Transform>();
            foreach (Transform t in children)
            {
                string lc = t.name.ToLower();
                if (lc.Contains("l") && lc.Contains("foot") && (ch.leftFoot == null || ch.leftFoot.name.Length > lc.Length))
                    ch.leftFoot = t;
                if (lc.Contains("r") && lc.Contains("foot") && (ch.rightFoot == null || ch.rightFoot.name.Length > lc.Length))
                    ch.rightFoot = t;
                if (lc.Contains("l") && lc.Contains("hand") && (ch.leftHand == null || ch.leftHand.name.Length > lc.Length))
                    ch.leftHand = t;
                if (lc.Contains("r") && lc.Contains("hand") && (ch.rightHand == null || ch.rightHand.name.Length > lc.Length))
                    ch.rightHand = t;
                if (lc.Contains("spine2") && (ch.chest == null || ch.chest.name.Length > lc.Length))
                    ch.chest = t;
                if (lc.Contains("head") && (ch.head == null || ch.head.name.Length > lc.Length))
                    ch.head = t;
            }
        }
    }
}
