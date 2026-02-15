using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedActions
{

    static DelayedActionsInScene delayedActionsInScene;


    public static void Do(Action action, float delay, MonoBehaviour executor, string actionName = "Action")
    {
        CheckDelayedActionsInScene();

        DelayedActionInfo info = new DelayedActionInfo
        {
            action = action,
            delay = delay,
#if UNITY_EDITOR
            actionName = actionName
#endif
        };
        delayedActionsInScene.AddAction(info, executor);

    }
    public static void RemoveActions(MonoBehaviour executor)
    {
        if (delayedActionsInScene == null)
            return;

        delayedActionsInScene.RemoveActions(executor);
    }
    private static void CheckDelayedActionsInScene()
    {
        if (delayedActionsInScene == null)
        {
            GameObject go = new GameObject("DelayedActions");
           
            delayedActionsInScene = go.AddComponent<DelayedActionsInScene>();
        }
    }
#if UNITY_EDITOR

    static DelayedActionsInScene delayedActionsInSceneB;

    public static void DoB(Action action, float delay, MonoBehaviour executor, string actionName = "Action")
    {
        CheckDelayedActionsInSceneB();
        DelayedActionInfo info = new DelayedActionInfo
        {
            action = action,
            delay = delay,

            actionName = actionName

        };
        delayedActionsInSceneB.AddAction(info, executor);

    }
    public static void RemoveActionsB(MonoBehaviour executor)
    {
        if (delayedActionsInSceneB == null)
            return;

        delayedActionsInSceneB.RemoveActions(executor);
    }
    private static void CheckDelayedActionsInSceneB()
    {
        if (delayedActionsInSceneB == null)
        {
            GameObject go = new GameObject("DelayedActions");
            delayedActionsInSceneB = go.AddComponent<DelayedActionsInScene>();
        }
    }
#endif

}
[Serializable]
internal class DelayedActionInfo
{
    [HideInInspector]
    public Action action;
    [ReadOnly]
#if UNITY_EDITOR
    public string actionName;
#endif
    public float delay;

}

#if UNITY_EDITOR
[Serializable]
internal class DelayedActionEntry
{
    public MonoBehaviour target;

    public List<DelayedActionInfo> actions = new();
}
[CustomEditor(typeof(DelayedActionsInScene))]
internal class DelayedActionsInSceneEditor : Editor
{
    //private Dictionary<MonoBehaviour, bool> foldoutStates = new();

    public override void OnInspectorGUI()
    {
    

        DrawDefaultInspector();

        // Botón de refresco manual
        if (GUILayout.Button("🔁 Debug Actions"))
        {
            DelayedActionsInScene script = (DelayedActionsInScene)target;
            script.DebugActions(); // Este método ya lo tienes
        }
    }
}

[CustomPropertyDrawer(typeof(DelayedActionEntry))]
internal class DelayedActionEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var targetProp = property.FindPropertyRelative("target");
        var actionsProp = property.FindPropertyRelative("actions");

        string targetName = targetProp.objectReferenceValue != null
            ? targetProp.objectReferenceValue.name
            : "Null Target";

        // Usa el nombre del objeto como label
        EditorGUI.PropertyField(position, property, new GUIContent(targetName), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
[CustomPropertyDrawer(typeof(DelayedActionInfo))]
internal class DelayedActionInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var actionNameProp = property.FindPropertyRelative("actionName");
        var delayProp = property.FindPropertyRelative("delay");

        float halfWidth = position.width / 2;

        // Campo: actionName (a la izquierda)
        var actionRect = new Rect(position.x, position.y, halfWidth - 5, position.height);
        EditorGUI.LabelField(actionRect, actionNameProp.stringValue);

        // Campo: delay (a la derecha)
        var delayRect = new Rect(position.x + halfWidth + 5, position.y, halfWidth - 5, position.height);
        EditorGUI.LabelField(delayRect, $"{delayProp.floatValue:F2}s");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
#endif