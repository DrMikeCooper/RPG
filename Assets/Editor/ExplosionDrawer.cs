using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    public class ExplosionDrawer : MonoBehaviour
    {

        [CustomEditor(typeof(Explosion))]
        public class PowerComboDrawer : Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("explosion"), new GUILayoutOption[0]);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

