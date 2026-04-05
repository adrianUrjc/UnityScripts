using System.Reflection;
using UnityEngine;

public class WebLoader : ALoader
{
    [SerializeField]
    private  string WEB_KEY = "web_group_values";
    void Start()
{
    Debug.Log("Saving test key...");
    PlayerPrefs.SetString("test_key", "hola_indexeddb");
    PlayerPrefs.Save();

    Debug.Log("Loaded: " + PlayerPrefs.GetString("test_key", "(no existe)"));
}


    // ----------------------------------------------------------------------
    // JSON LOAD override → lee desde PlayerPrefs
    // ----------------------------------------------------------------------
//     protected override void LoadFromJsonFile()
//     {
// #if UNITY_WEBGL && !UNITY_EDITOR
//         if (!PlayerPrefs.HasKey(WEB_KEY))
//         {
//             Debug.Log("[WebLoader] No existe JSON, creando valores por defecto.");
//             CreateJsonForWeb();
//             return;
//         }

//         string json = PlayerPrefs.GetString(WEB_KEY);
//         SerializableGroupSettings sgs = new SerializableGroupSettings();
//         JsonUtility.FromJsonOverwrite(json, sgs);
//         sgs.ApplyTo(values);

//         Debug.Log("[WebLoader] JSON cargado desde PlayerPrefs");
//        // PrintPlayerPrefsJson(); necesario para depurar unicamente
// #else
//         base.LoadFromJsonFile();
// #endif
//     }

    // ----------------------------------------------------------------------
    // JSON SAVE override → guarda en PlayerPrefs
    // ----------------------------------------------------------------------
//     protected override void SaveToJsonFile(string path=null,SerializableGroupSettings sgs=null)
//     {
// #if UNITY_WEBGL && !UNITY_EDITOR
//         SerializableGroupSettings sgs = new SerializableGroupSettings();
//         sgs.CopyFrom(values);

//         string json = JsonUtility.ToJson(sgs, true);

//         PlayerPrefs.SetString(WEB_KEY, json);
//         PlayerPrefs.Save();

//         Debug.Log("[WebLoader] JSON guardado en PlayerPrefs");
// #else
//         base.SaveToJsonFile();
// #endif
//     }

    // ----------------------------------------------------------------------
    // Crea datos iniciales en WebGL
    // ----------------------------------------------------------------------
   private void CreateJsonForWeb()
{
    if (values == null)
    {
        Debug.LogError("[WebLoader] 'values' es null, no se pueden crear datos iniciales.");
        return;
    }

    SerializableGroupValues sgs = new SerializableGroupValues();
    sgs.CopyFrom(values);

    string json = JsonUtility.ToJson(sgs, true);
    PlayerPrefs.SetString(WEB_KEY, json);
    PlayerPrefs.Save();

    Debug.Log("[WebLoader] JSON WEB creado");
}

[ContextMenu("Log All PlayerPrefs")]
    public  void LogAll()
    {
        Debug.Log("=== PLAYER PREFS ===");

        // PlayerPrefs usa una clase privada para guardar las keys en memoria
        var t = typeof(PlayerPrefs);
        var field = t.GetField("s_PlayerPrefsDict", BindingFlags.NonPublic | BindingFlags.Static);

        if (field == null)
        {
            Debug.LogWarning("No se puede acceder al diccionario interno de PlayerPrefs.");
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.LogError("En WebGL no es posible leer todas las claves. Debes leerlas una por una si conoces los nombres.");
#endif
            return;
        }

        var dict = field.GetValue(null) as System.Collections.IDictionary;

        if (dict == null)
        {
            Debug.LogWarning("PlayerPrefs está vacío.");
            return;
        }

        foreach (var key in dict.Keys)
        {
            string k = key.ToString();
            string v = PlayerPrefs.GetString(k, "[not string]");
            int i = PlayerPrefs.GetInt(k, int.MinValue);
            float f = PlayerPrefs.GetFloat(k, float.NaN);

            Debug.Log($"KEY: {k} | STR: {v} | INT: {i} | FLOAT: {f}");
        }

        Debug.Log("=====================");
    }
    public void PrintPlayerPrefsJson()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (PlayerPrefs.HasKey(WEB_KEY))
        {
            string json = PlayerPrefs.GetString(WEB_KEY);
            Debug.Log("===== PlayerPrefs JSON =====");
            Debug.Log(FormatJson(json));
            Debug.Log("============================");
        }
        else
        {
            Debug.Log("[PlayerPrefsDebugger] No hay datos guardados.");
        }
#else
        Debug.Log("[PlayerPrefsDebugger] Estás en Editor, puedes leer desde archivo local si quieres.");
#endif
    }

    private string FormatJson(string json)
    {
        try
        {
            var parsed = JsonUtility.FromJson<SerializableGroupValues>(json);
            return JsonUtility.ToJson(parsed, true); // con indentación
        }
        catch
        {
            return json; // si no es un JSON conocido, devuelve tal cual
        }
    }
}
