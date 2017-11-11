using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    [CustomEditor(typeof(PowerSetPassive))]
    public class PowerSetPassiveEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PowerSetPassive passive = (PowerSetPassive)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tint"));

            EditorGUILayout.LabelField("RPG Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
