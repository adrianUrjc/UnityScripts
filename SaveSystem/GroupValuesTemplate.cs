using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GroupValues/Master Template")]
public class GroupValuesTemplate : ScriptableObject
{
    public List<SettingField> defaultFields;
    public int version;
}