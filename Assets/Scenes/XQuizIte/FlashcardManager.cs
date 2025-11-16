using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

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

	public string PROMPT = "basic algebra"; // example; override from inspector

	public List<Flashcard> deck = new List<Flashcard>();

	public void SetPrompt(string newPrompt)
	{
		PROMPT = newPrompt;
		StartCoroutine(SendPrompt());
	}

	public void Start()
	{
		StartCoroutine(SendPrompt());
	}

	string LoadApiKey()
	{
		TextAsset keyFile = Resources.Load<TextAsset>("openai_key");

		if (keyFile == null)
		{
			Debug.LogError("API key file missing!");
			return null;
		}

		// You stored the reversed key in the file
		return keyFile.text.Trim();
	}

	public IEnumerator SendPrompt()
	{
		string apiKey = LoadApiKey();
		string url = "https://api.openai.com/v1/chat/completions";

		// Ask OpenAI for JSON list of flashcards
		string userRequest =
			$"Generate exactly 5 flashcards relating to this prompt: {PROMPT}. " +
			"Respond ONLY with valid JSON: an array of objects, each with keys 'question' and 'answer'.";

		string json =
		"{ " +
		"  \"model\": \"gpt-4o-mini\", " +
		"  \"messages\": [" +
		"    { \"role\": \"user\", \"content\": \"" + userRequest + "\" }" +
		"  ]" +
		"}";

		Debug.Log(json);

		UnityWebRequest req = new UnityWebRequest(url, "POST");
		byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
		req.uploadHandler = new UploadHandlerRaw(body);
		req.downloadHandler = new DownloadHandlerBuffer();
		req.SetRequestHeader("Content-Type", "application/json");
		req.SetRequestHeader("Authorization", "Bearer " + apiKey);

		yield return req.SendWebRequest();

		if (req.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("Request failed: " + req.error);
			yield break;
		}

		Debug.Log("Response received: " + req.downloadHandler.text);

		JObject obj = JObject.Parse(req.downloadHandler.text);
		string message = (string) (obj["choices"][0]["message"]["content"]);

		List<string> questions = splitBetween(message, "\"question\"", "\n");
		List<string> answers = splitBetween(message, "\"answer\"", "\n");

		Debug.Log(questions);
		Debug.Log(answers);

		deck.Clear();

		int count = Mathf.Min(questions.Count, answers.Count);

		for (int i = 0; i < count; i++)
		{
			deck.Add(new Flashcard
			{
				question = questions[i][3..^2],
				answer = answers[i][3..^1]
			});
		}
		SpawnStack();
	}

	List<string> splitBetween(string text, string splitA, string splitB)
	{
		List<string> results = new List<string>();
		int startIndex = 0;

		while (true)
		{
			// find the next A
			int aIndex = text.IndexOf(splitA, startIndex);
			if (aIndex == -1)
				break; // no more A → we're done

			// move to the end of A
			aIndex += splitA.Length;

			// find the next B after A
			int bIndex = text.IndexOf(splitB, aIndex);
			if (bIndex == -1)
				break; // found A but no B — malformed or last chunk

			// slice that delicious substring
			string between = text.Substring(aIndex, bIndex - aIndex);
			results.Add(between);

			// continue searching after B
			startIndex = bIndex + splitB.Length;
		}

		return results;
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
		float total = 0;
		float duration = 1f;
		float rotationSpeed = 180f; // degrees per second

		while (total < duration)
		{
			float delta = Time.deltaTime;
			total += delta;

			card.transform.Rotate(0f, rotationSpeed * delta, 0f);

			yield return null;
		}

		// Snap to nearest 90 degrees
		Vector3 euler = card.transform.eulerAngles;
		float snappedY = Mathf.Round(euler.y / 90f) * 90f;
		card.transform.eulerAngles = new Vector3(euler.x, snappedY, euler.z);

		view.ToggleSide();
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
