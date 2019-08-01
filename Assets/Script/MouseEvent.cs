using UnityEngine;
using UnityEngine.EventSystems;

abstract public class MouseEvent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
{
    abstract public void OnPointerEnter(PointerEventData eventData);

    abstract public void OnPointerUp(PointerEventData eventData);

    abstract public void OnPointerDown(PointerEventData eventData);

    abstract public void OnPointerExit(PointerEventData eventData);

    abstract public void OnPointerClick(PointerEventData eventData);
}
