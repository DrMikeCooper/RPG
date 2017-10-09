using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HideIfTypeAttribute))]
public class HideIfTypeDrawer : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (IsShowing(property))
        {
            return EditorGUIUtility.singleLineHeight;
        }
        else
        {
            return 0;
        }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (IsShowing(property))
            EditorGUI.PropertyField(position, property, true);
    }

    bool IsShowing(SerializedProperty property)
    {
        HideIfTypeAttribute hi = attribute as HideIfTypeAttribute;

        System.Type oType = property.serializedObject.targetObject.GetType();
        return (hi.hideType == oType) == hi.reverse;
    }
}
