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
                ch.FindBodyParts();
            }

            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onDead"), EditorStyles.standardFont);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onDamaged"), EditorStyles.standardFont);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStatusChanged"), EditorStyles.standardFont);
        }
    }
}
