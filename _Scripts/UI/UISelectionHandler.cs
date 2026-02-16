using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectionHandler : MonoBehaviour
{
    // Llamar a esto cuando cambie el tab
    public void OnTabChanged(GameObject newTab)
    {
        // Buscar el primer objeto seleccionable dentro del nuevo tab
        Selectable firstSelectable = newTab.GetComponentInChildren<Selectable>();

        if (firstSelectable != null)
        {
            // Asignar como seleccionado en el EventSystem
            EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
        }
    }
}
