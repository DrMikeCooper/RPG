using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG;

[CustomPropertyDrawer(typeof(RPGSettings.Tint))]
public class ColorCodeDrawer : PropertyDrawer
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
        var codeRect = new Rect(position.x, position.y, 100, position.height);
        var colorRect = new Rect(position.x + 105, position.y, 50, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(codeRect, property.FindPropertyRelative("code"), GUIContent.none);
        RPGSettings.ColorCode code = (RPGSettings.ColorCode)property.FindPropertyRelative("code").intValue;
        if (code == RPGSettings.ColorCode.Custom)
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("customColor"), GUIContent.none);
        else
        {
            EditorGUI.DrawRect(colorRect, Color.black);
            var colorRect2 = new Rect(position.x + 107, position.y+2, 46, position.height-4);
            RPGSettings.Tint col = new RPGSettings.Tint();
            col.code = code;        
            EditorGUI.DrawRect(colorRect2, col.GetColor());
        }
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

