
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager.UI;

#if UNITY_EDITOR

using UnityEditor;

using UnityEditor.Build.Reporting;
using UnityEditor.Build;
#endif

[System.Serializable]
public class SimpleScene
{
#if UNITY_EDITOR
    [CustomLabel("")]
    public SceneAsset sceneAsset;
#endif
    [SerializeField]
    private int index=-1;

    public int Index
    {
        get
        {
#if UNITY_EDITOR
            if (sceneAsset == null)
            {
                Debug.LogError("SceneAsset is null!");
                return -1;
            }

            var tempindex =SimpleSceneIndexer.ForceGetIndexOf(UnityEditor.AssetDatabase.GetAssetPath(sceneAsset));
            if (tempindex != index) {
                    Debug.LogWarning("The indexes of certain SimpleScenes are outdated, try refreshing them via Tools/SimpleScene - Refresh values");
             }
             index = tempindex;

#endif
            return index;

        }
    }
    public void RefreshIndex()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            index = SimpleSceneIndexer.ForceGetIndexOf(UnityEditor.AssetDatabase.GetAssetPath(sceneAsset));
        }
#endif
    }
}
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(SimpleScene))]
public class SimpleSceneDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty indexProp = property.FindPropertyRelative("index");
        SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
        EditorGUI.BeginChangeCheck();
        Object newValue = EditorGUI.ObjectField(
         position,
         label,
         sceneAssetProp.objectReferenceValue,
         typeof(SceneAsset),
         false
     );
      //  Debug.Log("Drawing SimpleScene property drawer.");

        if (EditorGUI.EndChangeCheck())
        {
            sceneAssetProp.objectReferenceValue = newValue;
            string scenePath = AssetDatabase.GetAssetPath(sceneAssetProp.objectReferenceValue);
            indexProp.intValue = SimpleSceneIndexer.ForceGetIndexOf(scenePath);

        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    void OnContentChanged(SerializedProperty property)
    {

    }
}
#endif


#if UNITY_EDITOR

public class SceneIndexBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Refreshing all SimpleScene indexes before build...");
        SimpleSceneIndexer.RefreshAll();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif

#if UNITY_EDITOR
public static class SimpleSceneIndexer
{
    public static int ForceGetIndexOf(string scenePath)
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
        if (buildIndex < 0)  //si la escena no esta en la lista se añade
        {

            var newScene = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettingsScene[] existingScenes = EditorBuildSettings.scenes;

            var updatedScenes = new EditorBuildSettingsScene[existingScenes.Length + 1];
            existingScenes.CopyTo(updatedScenes, 0);
            updatedScenes[existingScenes.Length] = newScene;
            EditorBuildSettings.scenes = updatedScenes;
            buildIndex = existingScenes.Length; // El nuevo índice será el último

            Debug.Log("Scene added to build settings: " + scenePath + " at index " + buildIndex);
        }
        return buildIndex;
    }
    public static void RefreshAll()
    {
        HashSet<Object> processedObjects = new HashSet<Object>();

        // 1️⃣ ScriptableObjects
        string[] soGuids = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (string guid in soGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so != null)
            {
                RefreshInObject(so, processedObjects);
            }
        }

        // 2️⃣ Prefabs
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                foreach (var mb in prefab.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    RefreshInObject(mb, processedObjects);
                }
            }
        }

        // 3️⃣ Open scenes (safe fallback)
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (var mb in root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    RefreshInObject(mb, processedObjects);
                }
            }
        }

        Debug.Log("SimpleScene index refresh completed.");
    }

    private static void RefreshInObject(Object obj, HashSet<Object> processed)
    {
        if (processed.Contains(obj))
            return;

        processed.Add(obj);
        SerializedObject serializedObject = new SerializedObject(obj);
        SerializedProperty prop = serializedObject.GetIterator();

        bool modified = false;

        while (prop.NextVisible(true))
        {
            if (prop.propertyType == SerializedPropertyType.Generic &&
                prop.type == nameof(SimpleScene))
            {
                SerializedProperty indexProp = prop.FindPropertyRelative("index");
                SerializedProperty sceneAssetProp = prop.FindPropertyRelative("sceneAsset");

                if (sceneAssetProp != null && sceneAssetProp.objectReferenceValue != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(sceneAssetProp.objectReferenceValue);
                    int newIndex = ForceGetIndexOf(scenePath);

                    if (indexProp.intValue != newIndex)
                    {
                        indexProp.intValue = newIndex;
                        modified = true;
                    }
                }
            }
        }

        if (modified)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(obj);
        }
    }
}
#endif
#if UNITY_EDITOR
public class SimpleSceneUpdateProject : EditorWindow
{

    private const float Width = 300f;
    private const float Height = 400f;
    [MenuItem("Tools/SimpleScene")]
    public static void Open()
    {
        var window = GetWindow<SimpleSceneUpdateProject>("SimpleScene - Refresh values");

        window.minSize = new Vector2(Width, Height);
        window.maxSize = new Vector2(Width, Height);
    }

    private void OnGUI()
    {
        GUILayout.Label("If loading a SimpleScene results");  
        GUILayout.Label("in a different Scene being loaded"); 
            GUILayout.Label("try refreshing.");
        if (GUILayout.Button("Refresh all simpleScene indexes"))
        {
            SimpleSceneIndexer.RefreshAll();
        }
    }
}
#endif