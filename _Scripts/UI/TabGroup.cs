using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI.Tabs
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] List<TabButton> tabButtons;
        [SerializeField] Sprite tabIdle;
        [SerializeField] Sprite tabHover;
        [SerializeField] Sprite tabActive;
        [SerializeField] List<GameObject> objectsToSwap;
        [SerializeField] UnityEvent<GameObject> TabChange;
        TabButton selectedTab;
        int currentIndex = 0;
        //void Start()
        //{
        //    if (tabButtons != null && tabButtons.Count > 0)
        //    {
        //        selectedTab = tabButtons[0];
        //        EventSystem.current.SetSelectedGameObject(selectedTab.gameObject);
        //        selectedTab.Select();
        //    }
        //}
        private void OnEnable()
        {
            if (tabButtons == null || tabButtons.Count == 0) return;


            var newTab = tabButtons[currentIndex];
            EventSystem.current.SetSelectedGameObject(newTab.gameObject);
            OnTabSelected(newTab);
        }
        public void Subscribe(TabButton button)
        {
            if (tabButtons == null)
                tabButtons = new List<TabButton>();

            tabButtons.Add(button);
            tabButtons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            if(button.transform.GetSiblingIndex() == 0)//si es el primero hijo seleccionar
            {
                OnTabSelected(button);
            }
        }

        // Este método lo conectas al action Navigate (tipo Value - Vector2) en el PlayerInput o InputAction asset
        public void OnNavigateTabs(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                float input = ctx.ReadValue<float>();

                // Detectamos el cambio solo cuando el input supera el umbral y antes no estaba presionado
                if (input > 0.5f)
                {
                    MoveSelection(1);
                }
                else if (input < -0.5f)
                {
                    MoveSelection(-1);
                }
            }
            // Aquí puedes añadir control vertical si quieres (por ejemplo, para grillas)
            // if (input.y > 0.5f && lastInput.y <= 0.5f) { MoveSelectionVertical(1); }
            // else if (input.y < -0.5f && lastInput.y >= -0.5f) { MoveSelectionVertical(-1); }

        }

        private void MoveSelection(int direction)
        {
            if (tabButtons == null || tabButtons.Count == 0) return;

            currentIndex = (currentIndex + direction + tabButtons.Count) % tabButtons.Count;

            var newTab = tabButtons[currentIndex];
            EventSystem.current.SetSelectedGameObject(newTab.gameObject);
            OnTabSelected(newTab);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            if (selectedTab == null || button != selectedTab)
                button.background.sprite = tabHover;
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton button)
        {
            if (selectedTab != null)
                selectedTab.Deselect();

            selectedTab = button;
            selectedTab.Select();
            ResetTabs();
            button.background.sprite = tabActive;

            int index = button.transform.GetSiblingIndex();
            for (int i = 0; i < objectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    
                    TabChange.Invoke(objectsToSwap[i]);
                }
                objectsToSwap[i].SetActive(i==index);
            }
        }

        public void ResetTabs()
        {
            foreach (TabButton button in tabButtons)
            {
                if (selectedTab != null && selectedTab == button) continue;
                button.background.sprite = tabIdle;
            }
        }
        
    }
}
