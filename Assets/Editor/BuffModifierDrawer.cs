using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG;

[CustomPropertyDrawer(typeof(Buff.Modifier))]
public class BuffModifierDrawer : PropertyDrawer
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
        var statRect = new Rect(position.x, position.y, 100, position.height);
        var valueRect = new Rect(position.x + 105, position.y, 50, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(statRect, property.FindPropertyRelative("stat"), GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("modifier"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

