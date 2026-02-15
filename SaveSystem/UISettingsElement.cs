//using Character.Settings.RebindUI;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace Character.Settings
{

    public class UISettingsElement : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] VALUE_TYPE dataType;
        public VALUE_TYPE DataType { get { return dataType; } }
        SettingsElementEventBase eventWrapper;
        public void Init()
        {
            switch (dataType)
            {
                case VALUE_TYPE.BOOL:
                    var boolEvent = new BoolSettingsElementEvent();
                    boolEvent.UIname = name;
                    eventWrapper = boolEvent;
                    var toggle = GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.onValueChanged.AddListener(boolEvent.InvokeEvent);
                        toggle.isOn = SettingsManager.Instance.GetValue<bool>(name);
                    }

                    break;

                case VALUE_TYPE.FLOAT:
                    var floatEvent = new FloatSettingsElementEvent();
                    floatEvent.UIname = name;
                    eventWrapper = floatEvent;
                    var slider = GetComponent<Slider>();
                    if (slider != null){
                    GetComponent<Slider>()?.onValueChanged.AddListener(floatEvent.InvokeEvent);
                    slider.value = SettingsManager.Instance.GetValue<float>(name);
                    }
                    break;

              
            }
        }
        
        public void Subscribe<T>(UnityAction<string, T> callback)
        {

            Debug.Log($"[Subscribe] Trying to subscribe {typeof(T)} to eventWrapper of type {eventWrapper?.GetType().ToString()}");
            if (eventWrapper == null)
            {
                Debug.Log($"[Subscribe] evenWrapper es nulo");
                return;
            }
            if (eventWrapper is SettingsElementEvent<T> typedEvent)
            {
                Debug.Log($"[Subscribe]Event subscribed of type: " + typeof(T));
                typedEvent.Subscribe(callback);
            }
            else
            {
                Debug.LogWarning($"Cannot subscribe: type mismatch for {typeof(T)} in {name}");
            }


        }



    }
    public abstract class SettingsElementEvent<T> : SettingsElementEventBase
    {
        public UnityEvent<string, T> onChangeSettingsElement = new UnityEvent<string, T>();
        public string UIname;
        public void InvokeEvent(T value)
        {
            //Debug.Log("[UISettingsElement] Invocando cambio de settings " + UIname + " : " + value.ToString());
            onChangeSettingsElement.Invoke(UIname, value);
        }
        public void Subscribe(UnityAction<string, T> listener)
        {
            onChangeSettingsElement.AddListener(listener);
        }

        public void Unsubscribe(UnityAction<string, T> listener)
        {
            onChangeSettingsElement.RemoveListener(listener);
        }
    }
    public class BoolSettingsElementEvent : SettingsElementEvent<bool> { }
    public class FloatSettingsElementEvent : SettingsElementEvent<float> { }
    public class StringSettingsElementEvent : SettingsElementEvent<string> { }
    public abstract class SettingsElementEventBase { }

}
