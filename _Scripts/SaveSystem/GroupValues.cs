using System;
using System.Collections;
using System.Collections.Generic;
using Character.Settings;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public enum VALUE_TYPE
{
    BOOL,
    FLOAT,
    DOUBLE,
    SHORT,
    INT,
    LONG,
    VECTOR2,
    STRING,
    BYTE,
}
[CreateAssetMenu(menuName = "ScriptableObject/GenericValues")]
public partial class GroupValues : ScriptableObject
{
    

    //TODO: optimizar con diccionario, de momento no funciona bien
    // private Dictionary<string, SettingEntry> _cache;

    // void BuildCache()
    // {
    //     _cache = new Dictionary<string, SettingEntry>();
    //     foreach (var field in fields)
    //         foreach (var entry in field.entries)
    //             _cache[entry.name] = entry;
    // }

    // void OnEnable() => BuildCache();
    // void OnDisable()
    // {
    //     _cache?.Clear();
    // }

    public List<SettingField> fields = new();
    public T GetValue<T>(string field, string name)
    {
        var f = fields.Find(f => f.fieldName == field);
        var entry = f?.entries.Find(e => e.name == name);
        return entry != null ? (T)entry.value.GetValue() : default;
    }
    public T GetValue<T>(string name)
    {
           foreach (var field in fields)
        {
            var entry = field.entries.Find(e => e.name == name);
            if (entry != null)
                return (T)entry.value.GetValue();
        }
        throw new KeyNotFoundException($"No se encontr� ning�n valor con el nombre '{name}' en los campos.");
    
    }



    public object GetValue(string name)
    {
        foreach (var field in fields)
        {
            var entry = field.entries.Find(e => e.name == name);
            if (entry != null)
                return entry.value.GetValue();
        }
        throw new KeyNotFoundException($"No se encontr� ning�n valor con el nombre '{name}' en los campos.");
    }
    public void SetValue<T>(string field, string name, T newValue)
    {

        var f = fields.Find(f => f.fieldName == field);
        var entry = f?.entries.Find(e => e.name == name);
        entry?.value.SetValue(newValue);
    }
    public void SetValue<T>(string name, T v)
    {
        if (typeof(T) == typeof(int) && v is float f)
            v = (T)(object)Mathf.RoundToInt(f);
        else
            v = (T)Convert.ChangeType(v, typeof(T));

          foreach (var field in fields)
        {
            var entry = field.entries.Find(e => e.name == name);
            if (entry != null)
            {
                entry.value.SetValue(v);
                return; // Salimos cuando encontramos y actualizamos el valor
            }
        }
        // Si quieres, aqu� podr�as lanzar excepci�n o log si no se encontr� el nombre
        Debug.LogError($"[GroupValues] No se encontr� ning�n valor con el nombre '{name}' en los campos.");
                throw new KeyNotFoundException($"No se encontr� ning�n valor con el nombre '{name}' en los campos.");
    


    }
    public void SetEntryValue(SettingEntry newEntry)
    {
        foreach (var field in fields)
        {
            var entry = field.entries.Find(e => e.name == newEntry.name);
            if (entry != null)
            {
                entry.value = newEntry.value;
                return;
            }
        }

        Debug.LogWarning($"[GroupValues] No se encontr� la entrada '{newEntry.name}' para actualizar.");
    }
    public void SetEntryValue(int fieldIndex, int entryIndex, object newValue)
    {
        if (fieldIndex >= 0 && fieldIndex < fields.Count &&
            entryIndex >= 0 && entryIndex < fields[fieldIndex].entries.Count)
        {
            fields[fieldIndex].entries[entryIndex].value = (SettingValue)newValue;
        }
        else
        {
            Debug.LogWarning("[GroupValues] �ndices fuera de rango al hacer SetEntryValue.");
        }
    }

    public GroupValues Clone()
    {
        var clone = ScriptableObject.CreateInstance<GroupValues>();
        clone.fields = new List<SettingField>();
        foreach (var field in fields)
        {
            clone.fields.Add(field.Clone());
        }
        return clone;
    }


    public void CopyFrom(GroupValues other)
    {
        fields.Clear();
        foreach (var field in other.fields)
        {
            fields.Add(field.Clone());
        }
    }
    public bool IsTheSame(GroupValues other)
    {
        if (other == null) return false;
        if (fields.Count != other.fields.Count) return false;

        foreach (var field in fields)
        {
            var otherField = other.fields.Find(f => f.fieldName == field.fieldName);
            if (otherField == null || field.entries.Count != otherField.entries.Count)
                return false;

            foreach (var e in field.entries)
            {
                var oe = otherField.entries.Find(x => x.name == e.name);
                if (oe == null || !e.Equals(oe)) return false;
            }
        }
        return true;
    }

    public void ResetToDefaults()
    {
        foreach (var field in fields)
            foreach (var entry in field.entries)
                entry.value = SettingValueFactory.Create(entry.type);
    }


}
#region INDIVIDUALELEMENT
[Serializable]
public abstract class SettingValue
{
    public abstract object GetValue();
    public abstract void SetValue(object value);
    public abstract VALUE_TYPE GetValueType();
    public abstract SettingValue Clone();

}

[Serializable]
public class SettingValue<T> : SettingValue
{
    public T value;

    public override object GetValue() => value;

    public override void SetValue(object val)
        => value = (T)Convert.ChangeType(val, typeof(T));

    public override VALUE_TYPE GetValueType()
        => SettingValueFactory.GetEnum(typeof(T));

    public override SettingValue Clone()
    {
        var clone = SettingValueFactory.Create(GetValueType());
        clone.SetValue(value);
        return clone;
    }

    public override bool Equals(object obj)
        => obj is SettingValue<T> other &&
           EqualityComparer<T>.Default.Equals(value, other.value);
}
#region SUBCLASSES
[Serializable] public class BoolSettingValue : SettingValue<bool> { }
[Serializable] public class IntSettingValue : SettingValue<int> { }
[Serializable] public class FloatSettingValue : SettingValue<float> { }
[Serializable] public class DoubleSettingValue : SettingValue<double> { }
[Serializable] public class LongSettingValue : SettingValue<long> { }
[Serializable] public class ShortSettingValue : SettingValue<short> { }
[Serializable] public class ByteSettingValue : SettingValue<byte> { }
[Serializable] public class StringSettingValue : SettingValue<string> { }
[Serializable] public class Vector2SettingValue : SettingValue<Vector2> { }


#endregion
public static class SettingValueFactory
{
    // public static SettingValue Create(VALUE_TYPE type) a Unity no le gustan los genericos en el inspector
    // {
    //     return type switch
    //     {
    //         VALUE_TYPE.BOOL => new SettingValue<bool>(),
    //         VALUE_TYPE.INT => new SettingValue<int>(),
    //         VALUE_TYPE.FLOAT => new SettingValue<float>(),
    //         VALUE_TYPE.DOUBLE => new SettingValue<double>(),
    //         VALUE_TYPE.LONG => new SettingValue<long>(),
    //         VALUE_TYPE.SHORT => new SettingValue<short>(),
    //         VALUE_TYPE.BYTE => new SettingValue<byte>(),
    //         VALUE_TYPE.STRING => new SettingValue<string>(),
    //         _ => throw new NotSupportedException()
    //     };
    // }
    public static SettingValue Create(VALUE_TYPE t)
    {
        return t switch
        {
            VALUE_TYPE.BOOL => new BoolSettingValue(),
            VALUE_TYPE.INT => new IntSettingValue(),
            VALUE_TYPE.FLOAT => new FloatSettingValue(),
            VALUE_TYPE.DOUBLE => new DoubleSettingValue(),
            VALUE_TYPE.LONG => new LongSettingValue(),
            VALUE_TYPE.SHORT => new ShortSettingValue(),
            VALUE_TYPE.BYTE => new ByteSettingValue(),
            VALUE_TYPE.STRING => new StringSettingValue(),
            VALUE_TYPE.VECTOR2 => new Vector2SettingValue(),
            _ => throw new NotSupportedException()
        };
    }

    public static VALUE_TYPE GetEnum(Type t)
    {
        if (t == typeof(bool)) return VALUE_TYPE.BOOL;
        if (t == typeof(int)) return VALUE_TYPE.INT;
        if (t == typeof(float)) return VALUE_TYPE.FLOAT;
        if (t == typeof(double)) return VALUE_TYPE.DOUBLE;
        if (t == typeof(long)) return VALUE_TYPE.LONG;
        if (t == typeof(short)) return VALUE_TYPE.SHORT;
        if (t == typeof(byte)) return VALUE_TYPE.BYTE;
        if (t == typeof(string)) return VALUE_TYPE.STRING;
        if (t == typeof(Vector2)) return VALUE_TYPE.VECTOR2;

        throw new NotSupportedException($"Tipo no soportado: {t}");
    }
}
#region ALTERNATIVE IMPLEMENTATIONS
// [Serializable]
// public class BoolSettingValue : SettingValue
// {
//     [SerializeField]
//     public bool value;

//     public override object GetValue() => value;
//     public override void SetValue(object val) => value = Convert.ToBoolean(val);
//     public override VALUE_TYPE GetValueType() => VALUE_TYPE.BOOL;
//     public BoolSettingValue()
//     {
//         value = false;
//     }
//     public override SettingValue Clone()
//     {
//         return new BoolSettingValue { value = this.value };
//     }
//     public override bool Equals(object obj)
//     {
//         if (obj is BoolSettingValue other) return value == other.value;
//         return false;
//     }
// }

// [Serializable]
// public class FloatSettingValue : SettingValue
// {
//     [SerializeField]
//     public float value;

//     public override object GetValue() => value;
//     public override void SetValue(object val) => value = Convert.ToSingle(val);
//     public override VALUE_TYPE GetValueType() => VALUE_TYPE.FLOAT;
//     public FloatSettingValue()
//     {
//         value = 0f;
//     }
//     public override SettingValue Clone()
//     {
//         return new FloatSettingValue { value = this.value };
//     }
//     public override bool Equals(object obj)
//     {
//         if (obj is FloatSettingValue other) return value == other.value;
//         return false;
//     }
// }

// [Serializable]
// public class StringSettingValue : SettingValue
// {
//     [SerializeField]
//     public string value;

//     public override object GetValue() => value;
//     public override void SetValue(object val) => value = val?.ToString();
//     public override VALUE_TYPE GetValueType() => VALUE_TYPE.STRING;
//     public StringSettingValue()
//     {
//         value = "";
//     }
//     public override SettingValue Clone()
//     {
//         return new StringSettingValue { value = this.value };
//     }
//     public override bool Equals(object obj)
//     {
//         if (obj is BoolSettingValue other) return value.Equals(other.value);
//         return false;
//     }

// }
#endregion
[Serializable]

public class SettingEntry
{
    public string name = "MyVariable";
    [CustomLabel("")]

    public VALUE_TYPE type;

    [SerializeReference] public SettingValue value;

    public SettingEntry Clone()
    {
        return new SettingEntry
        {
            name = name,
            type = type,
            value = value?.Clone()
        };
    }

    public override bool Equals(object obj)
    {
        if (obj is not SettingEntry other) return false;
        return name == other.name &&
               type == other.type &&
               Equals(value, other.value);
    }
}

#endregion


#region FIELD
[Serializable]
public class SettingField
{
    public string fieldName;
    public List<SettingEntry> entries = new();

    public SettingField Clone()
    {
        var f = new SettingField { fieldName = fieldName };
        foreach (var e in entries)
            f.entries.Add(e?.Clone());
        return f;
    }
}

#endregion