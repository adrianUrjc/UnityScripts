using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


internal class DelayedActionsInScene : MonoBehaviour
{

    private Dictionary<MonoBehaviour, List<DelayedActionInfo>> delayedActions = new();
#if UNITY_EDITOR
    [SerializeField]
    private bool debugMode = true; // Toggle for debug mode

    public List<DelayedActionEntry> DebugList => debugList;
    [SerializeField]
    private List<DelayedActionEntry> debugList = new();
#endif
    public void AddAction(DelayedActionInfo info, MonoBehaviour target)
    {
        if (delayedActions.ContainsKey(target))
        {
            delayedActions[target].Add(info);

        }
        else
        {
            delayedActions.Add(target, new List<DelayedActionInfo> { info });
        }
#if UNITY_EDITOR
        if (debugMode)
        {
            var entry = debugList.Find(e => e.target == target);
            if (entry == null)
            {
                entry = new DelayedActionEntry { target = target, actions = new List<DelayedActionInfo>() };
                debugList.Add(entry);
            }
            entry.actions.Add(info);
            Debug.Log($"Added action '{info.actionName}' with delay {info.delay} to target {target.name} ({target.GetType().Name})", target);
        }
#endif
    }
    public void RemoveActions(MonoBehaviour target)
    {
        delayedActions.Remove(target);
#if UNITY_EDITOR
        if (debugMode)
            debugList.RemoveAll(e => e.target == target);
#endif
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
    }
    // Update is called once per frame
    void Update()
    {
        var keys = new List<MonoBehaviour>(delayedActions.Keys);

        foreach (var id in keys)
        {
            if (!delayedActions.ContainsKey(id))
                continue;

            List<DelayedActionInfo> list = delayedActions[id];


            for (int i = list.Count - 1; i >= 0; i--) //recorrer la lista y eliminar las cosas que ya hayan terminado
            {
                if (!delayedActions.ContainsKey(id))
                    break;
                var actionInfo = list[i];

                actionInfo.delay -= Time.deltaTime;

                if (actionInfo.delay <= 0)
                {

                    try
                    {
                        actionInfo.action?.Invoke();

                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"DelayedActions. Error executing action '{actionInfo.actionName}' on target {id.name} ({id.GetType().Name}): {e.Message}. DelayedActions");
                    }

                    list.RemoveAt(i);
#if UNITY_EDITOR
                    if (debugMode)
                    {
                        var entry = debugList.Find(e => e.target == id);
                        if (entry != null)
                        {
                            entry.actions.Remove(actionInfo);
                            if (entry.actions.Count == 0)
                                debugList.Remove(entry);
                        }
                    }
#endif
                }



            }
            if (list.Count == 0) // Si la lista queda vacía, eliminar la entrada del diccionario
                delayedActions.Remove(id);

        }


    }
#if UNITY_EDITOR
    public void DebugActions()
    {
        foreach (var pair in delayedActions)
        {
            Debug.Log($"Target: {pair.Key.name} ({pair.Key.GetType().Name})", pair.Key);

            foreach (var actionInfo in pair.Value)
            {
                Debug.Log($" -> Action: {actionInfo.actionName}, Delay: {actionInfo.delay:F2}s", pair.Key);
            }
        }
    }
#endif
}