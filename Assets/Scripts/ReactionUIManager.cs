using UnityEngine;
using TMPro;
using System.Collections;

public class ReactionUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reactionUIText;

    private Coroutine fadeCoroutine;

    [SerializeField] private string introText = "Welcome to the Game!";
    private void Start()
    {
        DisplayReactionText(introText);
    }

    public void DisplayReactionText(string reactionText)
    {
        // Stop any existing fade coroutine to avoid overlap
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // Update the text and reset alpha to fully visible
        reactionUIText.text = reactionText;
        reactionUIText.color = new Color(reactionUIText.color.r, reactionUIText.color.g, reactionUIText.color.b, 1);

        // Start the fade coroutine
        fadeCoroutine = StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        yield return new WaitForSeconds(3f); // Wait before fading

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        Color originalColor = reactionUIText.color;

        // Gradually fade out the alpha
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            reactionUIText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure the text is fully transparent
        reactionUIText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        fadeCoroutine = null;
    }
}