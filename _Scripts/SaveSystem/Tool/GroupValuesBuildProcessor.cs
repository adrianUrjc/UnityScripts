#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
public class GroupValuesBuildProcessor //: IPreprocessBuildWithReport
{
    const string DEFAULT_FOLDER = "Assets/Resources/LoadSystem/SavedFiles/Default";

    public int callbackOrder => 0;
    static ALoader loader = new ALoader();
    static Dictionary<string, GroupValues> BuildDefaultLookup()
    {
        var dict = new Dictionary<string, GroupValues>();

        var all = GroupValuesRegistry.GetAll();

        foreach (var gv in all)
        {
            string path = AssetDatabase.GetAssetPath(gv);

            if (!IsInDefaultFolder(path))
                continue;

            dict[gv.name] = gv; // clave por nombre
        }

        return dict;
    }

    // public void OnPreprocessBuild(BuildReport report)
    // {
    //     Debug.Log("=== GroupValues Pre-Build Reset ===");

    //     var all = GroupValuesRegistry.GetAll();
    //     var defaultLookup = BuildDefaultLookup();

    //     foreach (var gv in all)
    //     {
    //         string path = AssetDatabase.GetAssetPath(gv);

    //         if (IsInDefaultFolder(path))
    //             continue;

    //         if (defaultLookup.TryGetValue(gv.name, out var defaultGV))
    //         {
    //             CopyValues(defaultGV, gv);
    //         }
    //         else
    //         {
    //             gv.ResetToDefaults();
    //         }

    //         EditorUtility.SetDirty(gv);
    //         ApplyJsonForGroupValues(gv);
    //     }

    //     AssetDatabase.SaveAssets();
    //     AssetDatabase.Refresh();

    //     PlayerPrefs.DeleteAll();
    //     PlayerPrefs.Save();

    //     Debug.Log("GroupValues processed with Default overrides");
    // }

    static void ApplyJsonForGroupValues(GroupValues gv)
    {
        string assetPath = AssetDatabase.GetAssetPath(gv);
        string folder = Path.GetDirectoryName(assetPath);
        string name = Path.GetFileNameWithoutExtension(assetPath);

        loader.ChangeAssetName(name);
        loader.SaveValues(gv);
        loader.SaveValues();
    }
    static bool IsInDefaultFolder(string assetPath)
    {
        return assetPath.Replace("\\", "/").StartsWith(DEFAULT_FOLDER);
    }
    static void CopyValues(GroupValues source, GroupValues target)
{
    
    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    var fields = typeof(GroupValues).GetFields(flags);

    foreach (var field in fields)
    {
        if (field.IsStatic) continue;

        var value = field.GetValue(source);
        field.SetValue(target, value);
    }
}
}
#endif
