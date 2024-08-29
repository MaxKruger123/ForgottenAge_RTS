using UnityEngine;
using UnityEngine.EventSystems;

public class UIDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseOver = false;

    // Called when the mouse enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    // Called when the mouse exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}
