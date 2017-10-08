using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG;

[CustomPropertyDrawer(typeof(AINode.AICondition))]
public class ConditionDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var typeRect = new Rect(position.x, position.y, 100, position.height);
        var labelRect = new Rect(position.x + 105, position.y, 25, position.height);
        var thresholdRect = new Rect(position.x + 140, position.y, 100, position.height);
        var reverseRect = new Rect(position.x + 240, position.y, 50, position.height);

        SerializedProperty typeProp = property.FindPropertyRelative("type");
        SerializedProperty reverseProp = property.FindPropertyRelative("reverse");
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);
        if (typeProp.intValue > 0)
        {
            if (typeProp.intValue == (int)AINode.AIConditionType.Status)
            {
                EditorGUI.LabelField(labelRect, reverseProp.boolValue ? "!=" : "==");
                EditorGUI.PropertyField(thresholdRect, property.FindPropertyRelative("status"), GUIContent.none);
            }
            else
            {
                EditorGUI.LabelField(labelRect, reverseProp.boolValue ? ">" : "<");
                EditorGUI.PropertyField(thresholdRect, property.FindPropertyRelative("threshold"), GUIContent.none);
            }
            EditorGUI.PropertyField(reverseRect, reverseProp, GUIContent.none);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

