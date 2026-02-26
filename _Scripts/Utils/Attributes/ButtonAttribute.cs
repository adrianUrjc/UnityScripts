using System;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : Attribute
{
    public string Label;

    public ButtonAttribute(string label = null)
    {
        Label = label;
    }
}