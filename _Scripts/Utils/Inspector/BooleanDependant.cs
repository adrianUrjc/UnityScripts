
#if UNITY_EDITOR

using UnityEditor;
#endif
using UnityEngine;

public class HideIfAttribute : PropertyAttribute
{
    public string booleanFieldName;
    public bool hiddenExpectedValue;
    public HideIfAttribute(string booleanFieldName, bool hiddenExpectedValue = true) 
    {
        this.booleanFieldName = booleanFieldName;
        this.hiddenExpectedValue = hiddenExpectedValue;
    }


}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HideIfAttribute))]
internal class BooleandependantDrawer : PropertyDrawer
{
    private bool IsVisible(SerializedProperty property, GUIContent label)
    {
        HideIfAttribute booleanDependant = (HideIfAttribute)attribute;
        string booleanFieldName = booleanDependant.booleanFieldName;
        bool hiddenExpectedValue = booleanDependant.hiddenExpectedValue;
        SerializedProperty booleanProperty = property.serializedObject.FindProperty(booleanFieldName);


        if (booleanProperty == null || booleanProperty.propertyType != SerializedPropertyType.Boolean)
        {


            Debug.LogWarning(label.text + " : BooleanDependantAttribute: No se ha encontrado la propiedad booleana con nombre " + booleanFieldName + " o no es booleana.");
            return  true; //Alguien se ha equivocado poniendo el nombre lol
        }

        //Todo va bien
        bool currentValue = booleanProperty.boolValue;
        return !(currentValue == hiddenExpectedValue);
       
   
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (IsVisible(property, label))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (IsVisible(property, label))
        {

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            return 0; //Para que no deje espacio
        }
    }
}
#endif