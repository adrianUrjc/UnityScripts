#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var targetType = target.GetType();
        var methods = targetType.GetMethods(
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
            if (buttonAttr == null) continue;

            string label = string.IsNullOrEmpty(buttonAttr.Label)
                ? method.Name
                : buttonAttr.Label;

            if (GUILayout.Button(label))
            {
                method.Invoke(target, null);
            }
        }
    }
}
#endif