using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollingBlock : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;

    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.enabled = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.enabled = true;
    }

    private void OnEnable()
    {
        scrollRect.enabled = true;
    }
}
