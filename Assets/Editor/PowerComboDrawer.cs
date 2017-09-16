using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG
{
    [CustomEditor(typeof(PowerCombo))]
    public class PowerComboDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            PowerCombo combo = (PowerCombo)target;

            serializedObject.Update();

            // Show default inspector property editor
            //DrawDefaultInspector();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("window"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("powers"), true, new GUILayoutOption[0]);

            serializedObject.ApplyModifiedProperties();

            string msg= "";

            for (int i = 0; i < combo.powers.Length; i++)
            {
                if (combo.powers[i].mode != Power.Mode.Instant)
                    msg += "WARNING!: Power " + combo.powers[i].name + " is not Instant!\n";
                if (combo.powers[i].closeToTargetSpeed != 0)
                    msg += "WARNING!: Power " + combo.powers[i].name + " is a Lunge!\n";
            }

            if (msg.Length > 0)
                EditorGUILayout.TextArea(msg, new GUILayoutOption[0]);
        }
    }
}
