using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SettingEntry))]
public class SettingEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var nameProp = property.FindPropertyRelative("name");
        var typeProp = property.FindPropertyRelative("type");
        var valueProp = property.FindPropertyRelative("value");

        float totalHeight = 0f;
        float lineHeight = EditorGUIUtility.singleLineHeight + 2f;

        // Altura para name y type
        totalHeight += lineHeight * 2;

        // Altura dinámica del campo "value"
        if (valueProp != null && valueProp.managedReferenceValue != null)
        {
            totalHeight += EditorGUI.GetPropertyHeight(valueProp, true) + 4f;
        }

        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty nameProp = property.FindPropertyRelative("name");
        SerializedProperty typeProp = property.FindPropertyRelative("type");
        SerializedProperty valueProp = property.FindPropertyRelative("value");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float y = position.y;

        // Campo "name"
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), nameProp);
        y += lineHeight + 2;

        // Campo "type" con detección de cambio
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), typeProp);
        if (EditorGUI.EndChangeCheck())
        {
            VALUE_TYPE selected = (VALUE_TYPE)typeProp.enumValueIndex;
            SettingValue newInstance = selected switch
            {
                VALUE_TYPE.BOOL => new BoolSettingValue(),
                VALUE_TYPE.FLOAT => new FloatSettingValue(),
                VALUE_TYPE.STRING => new StringSettingValue(),
                VALUE_TYPE.INT => new IntSettingValue(),
                VALUE_TYPE.DOUBLE => new DoubleSettingValue(),
                VALUE_TYPE.LONG => new LongSettingValue(),
                VALUE_TYPE.SHORT => new ShortSettingValue(),
                VALUE_TYPE.BYTE => new ByteSettingValue(),
                VALUE_TYPE.VECTOR2 => new Vector2SettingValue(),

                _ => null
            };

            if (newInstance != null)
            {
                valueProp.managedReferenceValue = newInstance;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
        y += lineHeight + 2;

        // Campo "value" (solo si hay instancia)
        if (valueProp != null && valueProp.managedReferenceValue != null)
        {
            float valueHeight = EditorGUI.GetPropertyHeight(valueProp, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, valueHeight), valueProp, true);
        }
    }
}

