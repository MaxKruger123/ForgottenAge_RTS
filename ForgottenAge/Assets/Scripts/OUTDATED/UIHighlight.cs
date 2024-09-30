using UnityEngine;
using UnityEngine.UI;

public class UIHighlight : MonoBehaviour
{
    public Image highlightImage;

    public void HighlightElement(RectTransform elementToHighlight)
    {
        highlightImage.gameObject.SetActive(true);
        highlightImage.rectTransform.position = elementToHighlight.position;
        highlightImage.rectTransform.sizeDelta = elementToHighlight.sizeDelta + new Vector2(10, 10);
    }

    public void RemoveHighlight()
    {
        highlightImage.gameObject.SetActive(false);
    }
}