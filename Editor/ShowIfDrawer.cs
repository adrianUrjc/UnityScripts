using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;

        if (ShouldShow(property, showIf))
            EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        return ShouldShow(property, showIf)
            ? EditorGUI.GetPropertyHeight(property, label)
            : 0;
    }

    private bool ShouldShow(SerializedProperty property, ShowIfAttribute showIf)
    {
        foreach (string boolName in showIf.conditionBools)
        {
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(boolName);
            if (conditionProperty == null || conditionProperty.propertyType != SerializedPropertyType.Boolean || !conditionProperty.boolValue)
                return false; // si alguna es false → no se muestra
        }
        return true; // todas true → se muestra
    }
}
