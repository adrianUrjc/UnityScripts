using UnityEngine;
using UnityEngine.Events;

public class EventPlayer : MonoBehaviour
{

    [SerializeField]
    private UnityEvent events;
    [SerializeField]
    bool playOnStart;
    [SerializeField]
    bool addDelay = false;
    [SerializeField, HideIf("addDelay", false)]
    float delay = 0.0f;

    private void Start()
    {
        if (playOnStart)
        {
            playEvents();
        }
    }
    public  void playEvents() {
        if (addDelay)
        {
            DelayedActions.Do(() =>
            {
                Debug.Log("Eventos ejecutados: " + gameObject.name);
                events?.Invoke();
            }, delay, this);
        }
        else
        {

            Debug.Log("Eventos ejecutados: " + gameObject.name);
            events?.Invoke();
        }
    }


}
