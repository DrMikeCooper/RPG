using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    [CustomEditor(typeof(RPGSettings))]
    public class RPGSettingsDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            RPGSettings targetSettings = (RPGSettings)target;

            // Show default inspector property editor
            DrawDefaultInspector();

            EditorGUILayout.LabelField("Standard Colours", EditorStyles.boldLabel);
            for (int i = 0; i < (int)RPGSettings.ColorCode.Custom; i++)
            {
                RPGSettings.ColorCode code = (RPGSettings.ColorCode)i;
                targetSettings.colors[i] = EditorGUILayout.ColorField(code.ToString(),targetSettings.colors[i], new GUILayoutOption[0]);
            }
        }
    }
}
