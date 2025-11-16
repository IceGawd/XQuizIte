using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FlashcardView : MonoBehaviour
{
	[Header("Card Faces")] public GameObject frontFace; // Question side
	public GameObject backFace; // Answer side

	[Header("Text Elements")] public TMP_Text frontText;
	public TMP_Text backText;

	[Header("Interaction Settings")] public float swipeThreshold = 0.3f;
	public float swipeTime = 0.25f;

	private bool showingFront = true;
	private Vector2 startPos;
	private float startTime;

	[Header("Events")] public UnityEvent onCardClicked;
	public UnityEvent onCardSwiped;

	void Start()
	{
		ShowFront();
	}

	public void SetText(string question, string answer)
	{
		frontText.text = question;
		backText.text = answer;
	}

	public void ToggleSide()
	{
		showingFront = !showingFront;
		frontFace.SetActive(showingFront);
		backFace.SetActive(!showingFront);
	}

	public void ShowFront()
	{
		showingFront = true;
		frontFace.SetActive(true);
		backFace.SetActive(false);
	}

	// XR/Mouse compatible interaction
	void OnMouseDown()
	{
		startPos = Input.mousePosition;
		startTime = Time.time;
	}

	void OnMouseUp()
	{
		Vector2 endPos = Input.mousePosition;
		float dt = Time.time - startTime;

		Vector2 delta = endPos - startPos;
		float dist = delta.magnitude;

		// Swipe
		if (dt <= swipeTime && dist >= swipeThreshold * Screen.width)
		{
			onCardSwiped.Invoke();
			return;
		}

		// Tap
		if (dist < Screen.width * 0.02f)
		{
			onCardClicked.Invoke();
		}
	}
}
