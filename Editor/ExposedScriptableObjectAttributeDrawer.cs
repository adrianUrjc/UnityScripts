using UnityEditor;
using UnityEngine;


// Custom Property Drawer para el atributo
[CustomPropertyDrawer(typeof(ExposedScriptableObjectAttribute))]
public class ExposedScriptableObjectAttributeDrawer : PropertyDrawer
{
    private Editor editor = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Dibujar etiqueta del campo
        EditorGUI.PropertyField(position, property, label, true);

        // Comprobar si el objeto tiene una referencia
        if (property.objectReferenceValue != null)
        {
            // Dibujar la flecha desplegable (foldout)
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);

            // Si está expandido, dibujar el inspector del ScriptableObject
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                if (editor == null)
                {
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
                }

                if (editor != null)
                {
                    editor.OnInspectorGUI();
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}
