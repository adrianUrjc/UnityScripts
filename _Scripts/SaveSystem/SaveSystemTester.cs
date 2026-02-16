using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemTester : MonoBehaviour
{
    [SerializeField] ALoader loader;
    [SerializeField] string valueName;
    [SerializeField] VALUE_TYPE valueType;
    [ContextMenu("Set Test Value")]
    void SetTestValue()
    {
        switch (valueType)
        {
            case VALUE_TYPE.BOOL:
                loader.SetValue<bool>(valueName, true);
                break;

            case VALUE_TYPE.INT:
                loader.SetValue<float>(valueName, 42.6f);
                break;

            case VALUE_TYPE.FLOAT:
                loader.SetValue<float>(valueName, 3.14f);
                break;

            case VALUE_TYPE.STRING:
                loader.SetValue<string>(valueName, "Hola");
                break;
        }
    }
    [ContextMenu("Get Test Value")]
    void GetTestValue()
    {
        switch (valueType)
        {
            case VALUE_TYPE.BOOL:
                bool boolValue = loader.GetValue<bool>(valueName);
                Debug.Log($"Bool Value: {boolValue}");
                break;

            case VALUE_TYPE.INT:
                int intValue = loader.GetValue<int>(valueName);
                Debug.Log($"Int Value: {intValue}");
                break;

            case VALUE_TYPE.FLOAT:
                float floatValue = loader.GetValue<float>(valueName);
                Debug.Log($"Float Value: {floatValue}");
                break;

            case VALUE_TYPE.STRING:
                string stringValue = loader.GetValue<string>(valueName);
                Debug.Log($"String Value: {stringValue}");
                break;
        }

    }
}
