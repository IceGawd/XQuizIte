using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class FlashcardView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Header("Card Faces")] 
	public GameObject frontFace;
	public GameObject backFace;

	[Header("Text Elements")] 
	public TMP_Text frontText;
	public TMP_Text backText;

	[Header("Interaction Settings")] 
	public float swipeThreshold = 0.025f;  // % of screen width
	public float swipeTime = 0.075f;

	private bool showingFront = true;
	private Vector2 startPos;
	private float startTime;

	[Header("Events")] 
	public UnityEvent onCardClicked;
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

	// XR + Mouse + Touch compatible input
	public void OnPointerDown(PointerEventData eventData)
	{
		startPos = eventData.position;
		startTime = Time.time;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Vector2 endPos = eventData.position;
		float dt = Time.time - startTime;

		Vector2 delta = endPos - startPos;
		float dist = delta.magnitude;

		Debug.Log("Pointer Up");
		Debug.Log(dt);
		Debug.Log(dist);
		Debug.Log(Screen.width);

		// Swipe detection
		if (dist >= swipeThreshold * Screen.width)
		{
			onCardSwiped.Invoke();
			return;
		}

		// Tap detection
		if (dt <= swipeTime)
		{
			onCardClicked.Invoke();
			return;
		}
	}
}
