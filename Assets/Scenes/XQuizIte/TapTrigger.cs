using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TapTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("Event to invoke when this object is tapped/clicked.")]
    public UnityEvent onTap;

    private bool pressedInside = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressedInside = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pressedInside)
        {
            onTap?.Invoke();
        }
        pressedInside = false;
    }
}
