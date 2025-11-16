using UnityEngine;
using TMPro;

public class PromptInputUI : MonoBehaviour
{
    public FlashcardManager manager;

    private TouchScreenKeyboard keyboard;
    private string initialText;

    void Update()
    {
        // Poll for keyboard completion
        if (keyboard != null && keyboard.active == false)
        {
            if (!keyboard.wasCanceled)
            {
                string result = keyboard.text;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    manager.SetPrompt(result);
                }
            }

            keyboard = null;
        }
    }

    // Called by your XR prompt cube/button
    public void OpenKeyboard()
    {
        initialText = manager.PROMPT;

        keyboard = TouchScreenKeyboard.Open(
            initialText,
            TouchScreenKeyboardType.Default,
            false,  // autocorrect
            false,  // multiline
            false,  // secure
            false,  // alert
            "Enter flashcard topic"
        );
    }
}
