using System;

public interface ISettingsContainer
{
    public void SubscribeToSettingsChange(Action onChange);
    public T GetValue<T>(string key);
    public void SetValue<T>(string key,T value);
}
