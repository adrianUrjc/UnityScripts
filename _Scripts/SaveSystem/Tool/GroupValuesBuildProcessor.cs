#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Reflection;
using UnityEngine;

public class GroupValuesBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
   static ALoader loader = new ALoader();

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("=== GroupValues Pre-Build Reset ===");

        var all = GroupValuesRegistry.GetAll();

        foreach (var gv in all)
        {
            gv.ResetToDefaults();
            EditorUtility.SetDirty(gv);

            ApplyJsonForGroupValues(gv);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Editor PlayerPrefs (opcional)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("GroupValues reset + JSON regenerated + PlayerPrefs cleared (Editor)");
    }

    static void ApplyJsonForGroupValues(GroupValues gv)
    {
        string assetPath = AssetDatabase.GetAssetPath(gv);
        string folder = Path.GetDirectoryName(assetPath);
        string name = Path.GetFileNameWithoutExtension(assetPath);

        loader.ChangeAssetName(name);

        typeof(ALoader)
            .GetField("values", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(loader, gv);

        loader.SaveValues();
    }
}
#endif
