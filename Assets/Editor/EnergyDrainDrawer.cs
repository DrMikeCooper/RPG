using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    public class EnergyDrainDrawer : MonoBehaviour
    {

        [CustomEditor(typeof(EnergyDrain))]
        public class PowerComboDrawer : Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("drain"), new GUILayoutOption[0]);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
