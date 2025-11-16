using UnityEngine;
using UnityEngine.EventSystems;

public class PromptInputObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public PromptInputUI inputUI;  // drag in inspector
	private bool pressedInside = false;

	public void OnPointerDown(PointerEventData eventData)
	{
		pressedInside = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (pressedInside)
		{
			inputUI.OpenKeyboard();
		}
		pressedInside = false;
	}
}
