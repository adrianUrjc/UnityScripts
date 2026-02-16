//using Character.Settings.RebindUI;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace Character.Settings
{
    //TODO: input field and strings
    public enum UIElement { /*BUTTON,*/ TOGGLE, SLIDER, DRAWER, TMP_DRAWER }

    public class UISettingsElement : MonoBehaviour, ILoaderUser
    {
        // Start is called before the first frame update
        [SerializeField]
        [Tooltip("String that gets the value")]
        string keyValue = "";
        [SerializeField] VALUE_TYPE dataType;
        [SerializeField] UIElement uIElement;

        public VALUE_TYPE DataType { get { return dataType; } }
        #region INSPECTOR
        public void Reset()
        {
            GetUIElementType();
        }
        public void GetUIElementType()
        {

            if (TryGetComponent<Slider>(out _))
            {
                dataType = VALUE_TYPE.FLOAT;
                uIElement = UIElement.SLIDER;
            }

            else if (TryGetComponent<Toggle>(out _))
            {

                dataType = VALUE_TYPE.BOOL;
                uIElement = UIElement.TOGGLE;
            }
            else if (TryGetComponent<Dropdown>(out _))
            {
                dataType = VALUE_TYPE.INT;
                uIElement = UIElement.DRAWER;
            }
            else if (TryGetComponent<TMP_Dropdown>(out _))
            {
                dataType = VALUE_TYPE.INT;
                uIElement = UIElement.TMP_DRAWER;
            }

            else
            {
                Debug.LogWarning("[UISettingsElement]Interface element not recognized");
            }
        }
        #endregion
        void OnEnable()
        {
            Init();
        }
        public void Init()
        {
            var container = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISettingsContainer>()
            .FirstOrDefault();
            if (container != null)
            {
                Debug.Log("Encontrado cambiando valores");
                container.SubscribeToSettingsChange(OnValuesChange);
                OnValuesChange();
            }
        }

        public void SubscribeToValuesChange()
        {
            var container = FindFirstObjectByType<MonoBehaviour>() as ISettingsContainer;
            container.SubscribeToSettingsChange(OnValuesChange);
        }
        //To save data
        [ContextMenu("Save values")]
        public void SetValues()
        {
            string kValue = keyValue != "" ? keyValue : name;
            var container = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
           .OfType<ISettingsContainer>()
           .FirstOrDefault();

            switch (uIElement)
            {
                case UIElement.TOGGLE:
                    var toggle = GetComponent<Toggle>();
                    if (toggle)
                    {
                        container.SetValue<bool>(kValue, toggle.isOn);
                    }
                    break;
                case UIElement.SLIDER:
                    var slider = GetComponent<Slider>();
                    if (slider)
                    {

                        container.SetValue<float>(kValue, slider.value);
                    }
                    break;
                case UIElement.DRAWER:
                    var dropdown = GetComponent<Dropdown>();
                    if (dropdown)
                    {

                        container.SetValue<int>(kValue, dropdown.value);
                    }
                    break;
                case UIElement.TMP_DRAWER:
                    var dropdownTM = GetComponent<TMP_Dropdown>();
                    if (dropdownTM)
                    {

                        container.SetValue<int>(kValue, dropdownTM.value);
                    }
                    break;
            }
        }
        //To load data
        public void OnValuesChange()
        {
            string kValue = keyValue != "" ? keyValue : name;
            var container = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
           .OfType<ISettingsContainer>()
           .FirstOrDefault();

            switch (uIElement)
            {
                case UIElement.TOGGLE:
                    var toggle = GetComponent<Toggle>();
                    if (toggle)
                    {

                        toggle.isOn = container.GetValue<bool>(kValue);
                    }
                    break;
                case UIElement.SLIDER:
                    var slider = GetComponent<Slider>();
                    if (slider)
                    {

                        slider.value = container.GetValue<float>(kValue);
                    }
                    break;
                case UIElement.DRAWER:
                    var dropdown = GetComponent<Dropdown>();
                    if (dropdown)
                    {

                        dropdown.value = container.GetValue<int>(kValue);
                    }
                    break;
                case UIElement.TMP_DRAWER:
                    var dropdownTM = GetComponent<TMP_Dropdown>();
                    if (dropdownTM)
                    {

                        dropdownTM.value = container.GetValue<int>(kValue);
                    }
                    break;
            }

        }


    }


}
