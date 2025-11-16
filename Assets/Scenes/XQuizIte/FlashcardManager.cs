using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlashcardManager : MonoBehaviour
{
	[Header("Prefabs & Positions")]
	public GameObject flashcardPrefab;           // Card prefab with front/back
	public Transform stackRoot;                  // Where cards spawn/stack
	public float stackOffset = 0.1f;            // Slight downward offset per card

	[Header("Animation Settings")]
	public float flipSpeed = 4f;
	public float swipeSpeed = 2f;
	public Vector3 swipeDirection = new Vector3(1, 0.2f, 0); // Swipe off-screen dir

	private List<GameObject> cardObjects = new List<GameObject>();

	// Flashcard data structure
	[System.Serializable]
	public class Flashcard
	{
		public string question;
		public string answer;
	}

	public List<Flashcard> deck = new List<Flashcard>();

	void Start()
	{
		// Example: Populate deck manually; replace with GPT-generated content
		deck.Add(new Flashcard { question = "What is 2 + 2?", answer = "4" });
		deck.Add(new Flashcard { question = "Capital of France?", answer = "Paris" });
		deck.Add(new Flashcard { question = "Speed of light?", answer = "3e8 m/s" });

		SpawnStack();

		// // Generate a card at every integer coordinate in [-1,1]^3
		// for (int x = -1; x <= 1; x++)
		// {
		// 	for (int y = -1; y <= 1; y++)
		// 	{
		// 		for (int z = -1; z <= 1; z++)
		// 		{
		// 			GameObject card = Instantiate(flashcardPrefab);
		// 			card.transform.position = Vector3.zero;

		// 			// position inside the cube
		// 			card.transform.localPosition = new Vector3(x, y, z);

		// 			// label with coords
		// 			FlashcardView view = card.GetComponent<FlashcardView>();
		// 			string coords = $"({x}, {y}, {z})";
		// 			view.SetText(coords, coords); // front and back same for debugging

		// 			// interaction handlers
		// 			view.onCardClicked.AddListener(() => StartCoroutine(FlipCard(card, view)));
		// 			view.onCardSwiped.AddListener(() => StartCoroutine(SwipeCard(card)));

		// 			cardObjects.Add(card);
		// 		}
		// 	}
		// }

	}

	public void SpawnStack()
	{
		foreach (GameObject g in cardObjects)
			Destroy(g);
		cardObjects.Clear();

		for (int i = 0; i < deck.Count; i++)
		{
			GameObject card = Instantiate(flashcardPrefab, stackRoot);
			card.transform.localPosition = new Vector3(0, -i * stackOffset, i * stackOffset);

			FlashcardView view = card.GetComponent<FlashcardView>();
			view.SetText(deck[i].question, deck[i].answer);
			view.onCardClicked.AddListener(() => StartCoroutine(FlipCard(card, view)));
			view.onCardSwiped.AddListener(() => StartCoroutine(SwipeCard(card)));

			cardObjects.Add(card);
		}
	}

	IEnumerator FlipCard(GameObject card, FlashcardView view)
	{
		while (true)
		{
			// Rotate around the Y axis at 45 degrees per second
			card.transform.Rotate(0f, 45f * Time.deltaTime, 0f);

			yield return null;
		}
	}


	IEnumerator SwipeCard(GameObject card)
	{
		Vector3 start = card.transform.position;
		Vector3 end = start + swipeDirection * 2f;

		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * swipeSpeed;
			card.transform.position = Vector3.Lerp(start, end, t);
			yield return null;
		}
	}
}
