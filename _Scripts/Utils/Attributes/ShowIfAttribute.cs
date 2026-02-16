using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string[] conditionBools;

    public ShowIfAttribute(params string[] conditionBools)
    {
        this.conditionBools = conditionBools;
    }
}
