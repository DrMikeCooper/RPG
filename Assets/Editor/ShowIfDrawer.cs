using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
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
        ShowIfAttribute si = attribute as ShowIfAttribute;
        SerializedProperty prop = property.serializedObject.FindProperty(si.varName);
        bool show = true;
        if (prop != null)
        {
            if (prop.propertyType == SerializedPropertyType.Boolean)
            {
                switch (si.comparison)
                {
                    case ShowIfAttribute.Comparison.Equals: show = prop.boolValue; break;
                    case ShowIfAttribute.Comparison.Not: show = !prop.boolValue; break;
                }
            }
            if (prop.propertyType == SerializedPropertyType.Integer || prop.propertyType == SerializedPropertyType.Enum)
            {
                switch (si.comparison)
                {
                    case ShowIfAttribute.Comparison.Equals:  show = prop.intValue == si.threshold; break;
                    case ShowIfAttribute.Comparison.Not:     show = prop.intValue != si.threshold; break;
                    case ShowIfAttribute.Comparison.Greater: show = prop.intValue > si.threshold; break;
                    case ShowIfAttribute.Comparison.Less:    show = prop.intValue < si.threshold; break;
                }
            }

            if (prop.propertyType == SerializedPropertyType.Float)
            {
                switch (si.comparison)
                {
                    case ShowIfAttribute.Comparison.Equals:  show = prop.floatValue == si.threshold; break;
                    case ShowIfAttribute.Comparison.Not:     show = prop.floatValue != si.threshold; break;
                    case ShowIfAttribute.Comparison.Greater: show = prop.floatValue > si.threshold; break;
                    case ShowIfAttribute.Comparison.Less:    show = prop.floatValue < si.threshold; break;
                }
            }
            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                switch (si.comparison)
                {
                    case ShowIfAttribute.Comparison.Not: show = prop.objectReferenceValue == null; break;
                    default:                             show = prop.objectReferenceValue != null; break;
                }
            }
        }
        return show;
    }
}
