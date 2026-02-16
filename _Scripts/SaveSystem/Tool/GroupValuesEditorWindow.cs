#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class GroupValuesWindow : EditorWindow
{
    ALoader loader = new ALoader();
    private List<GroupValues> allValues;
    private int selectedIndex = -1;

    private GroupValues selected;
    private GroupValues workingCopy;
    private GroupValues originalCopy;

    private Vector2 scroll;

    [MenuItem("Tools/Group Values Window")]
    public static void Open()
    {
        GetWindow<GroupValuesWindow>("GroupValues");
    }

    void OnEnable()
    {
        RefreshRegistry();
    }

    void RefreshRegistry()
    {
        allValues = GroupValuesRegistry.GetAll();
        Repaint();
    }

    void OnGUI()
    {
        DrawRegistryPanel();

        if (selected == null)
            return;

        DrawToolbar();
        DrawEditor();
    }

    // =========================================================
    // REGISTRY PANEL
    // =========================================================
    void DrawRegistryPanel()
    {
        EditorGUILayout.LabelField("GROUP VALUES REGISTRY", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh List"))
            RefreshRegistry();

        string[] names = allValues.ConvertAll(v => v.name).ToArray();

        int newIndex = EditorGUILayout.Popup("Selected Asset", selectedIndex, names);

        if (newIndex != selectedIndex && newIndex >= 0)
        {
            selectedIndex = newIndex;
            Select(allValues[selectedIndex]);
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create GroupValues"))
            CreateNewGroupValues();

        if (selected != null && GUILayout.Button("Delete Selected"))
            DeleteSelected();
        if (GUILayout.Button("Reset All"))
            ResetAllGroupValuesAndApply();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
    void ResetAllGroupValuesAndApply()
    {
        if (!EditorUtility.DisplayDialog(
            "Reset ALL GroupValues",
            "This will reset ALL GroupValues to defaults and overwrite JSON.\nThis cannot be undone.",
            "Yes", "Cancel"))
            return;

        var all = GroupValuesRegistry.GetAll();

        foreach (var gv in all)
        {
            gv.ResetToDefaults();
            EditorUtility.SetDirty(gv);

            // Rebuild JSON
            ApplyJsonForGroupValues(gv);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("All GroupValues reset and JSON regenerated");
    }
    void ApplyJsonForGroupValues(GroupValues gv)
    {
        string assetPath = AssetDatabase.GetAssetPath(gv);
        string folder = Path.GetDirectoryName(assetPath);
        string name = Path.GetFileNameWithoutExtension(assetPath);

        loader.ChangeAssetName(name);

        //Hack: asignar el SO manualmente
        typeof(ALoader)
            .GetField("values", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(loader, gv);

        loader.SaveValues();
    }

    void Select(GroupValues gv)
    {
        selected = gv;
        originalCopy = gv.Clone();
        workingCopy = gv.Clone();
    }

    // =========================================================
    // TOOLBAR
    // =========================================================
    void DrawToolbar()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Copy From Current"))
            DuplicateCurrentGroupValues();


        if (GUILayout.Button("Apply"))
            Apply();

        if (GUILayout.Button("Undo"))
            Undo();

        if (GUILayout.Button("Reset To Defaults"))
            workingCopy.ResetToDefaults();

        GUILayout.EndHorizontal();
    }

    // =========================================================
    // EDITOR UI
    // =========================================================
    void DrawEditor()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int i = 0; i < workingCopy.fields.Count; i++)
        {
            var field = workingCopy.fields[i];

            EditorGUILayout.BeginVertical("box");

            // Field header
            EditorGUILayout.BeginHorizontal();
            field.fieldName = EditorGUILayout.TextField("Field Name", field.fieldName);

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                workingCopy.fields.RemoveAt(i);
                GUI.backgroundColor = Color.white;
                break;
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            // Entries
            for (int j = 0; j < field.entries.Count; j++)
            {
                DrawEntry(field, j);
            }

            if (GUILayout.Button("+ Add Entry"))
            {
                AddEntry(field);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("+ Add Field"))
        {
            workingCopy.fields.Add(new SettingField()
            {
                fieldName = "NewField"
            });
        }

        EditorGUILayout.EndScrollView();
    }

    void DrawEntry(SettingField field, int index)
    {
        var entry = field.entries[index];
        if (field.entries.Exists(x => x != entry && x.name == entry.name))
            EditorGUILayout.HelpBox("Duplicate key!", MessageType.Error);


        EditorGUILayout.BeginHorizontal();

        entry.name = EditorGUILayout.TextField(entry.name, GUILayout.Width(150));

        // Type selector
        var newType = (VALUE_TYPE)EditorGUILayout.EnumPopup(entry.type, GUILayout.Width(80));
        if (newType != entry.type)
        {
            entry.type = newType;
            entry.value = SettingValueFactory.Create(newType); // recreate value
        }

        DrawEntryValue(entry);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("X", GUILayout.Width(20)))
        {
            field.entries.RemoveAt(index);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            return;
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();

    }
    void DrawEntryValue(SettingEntry entry)
    {
        switch (entry.type)
        {
            case VALUE_TYPE.BOOL:
                ((BoolSettingValue)entry.value).value =
                    EditorGUILayout.Toggle(((BoolSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.INT:
                ((IntSettingValue)entry.value).value =
                    EditorGUILayout.IntField(((IntSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.FLOAT:
                ((FloatSettingValue)entry.value).value =
                    EditorGUILayout.FloatField(((FloatSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.DOUBLE:
                ((DoubleSettingValue)entry.value).value =
                    EditorGUILayout.DoubleField(((DoubleSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.LONG:
                ((LongSettingValue)entry.value).value =
                    EditorGUILayout.LongField(((LongSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.SHORT:
                ((ShortSettingValue)entry.value).value =
                    (short)EditorGUILayout.IntField(((ShortSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.BYTE:
                ((ByteSettingValue)entry.value).value =
                    (byte)EditorGUILayout.IntField(((ByteSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.STRING:
                ((StringSettingValue)entry.value).value =
                    EditorGUILayout.TextField(((StringSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.VECTOR2:
                ((Vector2SettingValue)entry.value).value =
                    EditorGUILayout.Vector2Field("", ((Vector2SettingValue)entry.value).value);
                break;
        }
    }

    void AddEntry(SettingField field)
    {
        var e = new SettingEntry();
        e.name = "NewEntry_" + field.entries.Count;

        e.type = VALUE_TYPE.INT;
        e.value = SettingValueFactory.Create(e.type);

        field.entries.Add(e);
        Apply();


    }

    // 

    // {
    //     var all = GroupValuesRegistry.GetAll();

    //     foreach (var gv in all)
    //     {
    //         if (gv == selected)
    //             continue;

    //         bool changed = false;

    //         foreach (var field in workingCopy.fields)
    //         {
    //             var targetField = gv.fields.Find(f => f.fieldName == field.fieldName);
    //             if (targetField == null)
    //             {
    //                 gv.fields.Add(field.Clone());
    //                 changed = true;
    //                 continue;
    //             }

    //             // Propagate entries
    //             foreach (var entry in field.entries)
    //             {
    //                 var targetEntry = targetField.entries.Find(e => e.name == entry.name);
    //                 if (targetEntry == null)
    //                 {
    //                     targetField.entries.Add(entry.Clone());
    //                     changed = true;
    //                 }
    //             }
    //         }

    //         if (changed)
    //         {
    //             EditorUtility.SetDirty(gv);
    //         }
    //     }

    //     AssetDatabase.SaveAssets();
    //     Debug.Log("Schema propagated to all GroupValues");
    // }

    // =========================================================
    // APPLY / UNDO
    // =========================================================
    void DuplicateCurrentGroupValues()
    {
        if (selected == null)
            return;

        string path = AssetDatabase.GetAssetPath(selected);
        string newPath = AssetDatabase.GenerateUniqueAssetPath(path.Replace(".asset", "_Copy.asset"));

        GroupValues copy = ScriptableObject.CreateInstance<GroupValues>();
        copy.CopyFrom(selected);

        AssetDatabase.CreateAsset(copy, newPath);
        AssetDatabase.SaveAssets();

        Debug.Log("GroupValues duplicated: " + newPath);
    }

    void Apply()
    {
        selected.CopyFrom(workingCopy);

        EditorUtility.SetDirty(selected);
        AssetDatabase.SaveAssets();

        // Guardar JSON con ALoader
        SaveJsonForAsset(selected);

        originalCopy = selected.Clone();
        Debug.Log("Applied changes + JSON updated");
    }
    void SaveJsonForAsset(GroupValues asset)
    {
        string assetPath = AssetDatabase.GetAssetPath(asset);
        string folder = Path.GetDirectoryName(assetPath);
        string name = Path.GetFileNameWithoutExtension(assetPath);

        loader.ChangeAssetName(name);

        // // Override path
        // typeof(ALoader).GetField("soPath", 
        //     System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        //     ?.SetValue(loader, folder + "/");

        // // Set values manually
        // typeof(ALoader).GetField("values", 
        //     System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        //     ?.SetValue(loader, asset);

        loader.SaveValues(asset);
    }


    void Undo()
    {
        workingCopy = originalCopy.Clone();
        Debug.Log("Undo changes");
    }

    // =========================================================
    // CREATE / DELETE
    // =========================================================
    void CreateNewGroupValues()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create GroupValues",
            "NewGroupValues",
            "asset",
            "Save GroupValues"
        );

        if (string.IsNullOrEmpty(path))
            return;

        var gv = ScriptableObject.CreateInstance<GroupValues>();
        AssetDatabase.CreateAsset(gv, path);
        AssetDatabase.SaveAssets();

        RefreshRegistry();
    }

    void DeleteSelected()
    {
        if (!EditorUtility.DisplayDialog("Delete GroupValues?",
            $"Delete {selected.name}?\nThis cannot be undone.",
            "Yes", "Cancel"))
            return;

        string path = AssetDatabase.GetAssetPath(selected);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();

        selected = null;
        workingCopy = null;
        originalCopy = null;
        selectedIndex = -1;

        RefreshRegistry();
    }
}
#endif
