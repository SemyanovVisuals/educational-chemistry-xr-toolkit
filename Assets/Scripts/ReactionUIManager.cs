using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ReactionUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reactionUIText;
    [SerializeField] private Image reactionUIImage;
    private Coroutine fadeCoroutine;

    [SerializeField] private string introText = "Welcome to the Game!";
    private void Start()
    {
        if(introText !="")
        {
            DisplayReactionText(introText);

        }
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

        // Update the image alpha to fully visible
        if (reactionUIImage != null)
        {
            reactionUIImage.color = new Color(reactionUIImage.color.r, reactionUIImage.color.g, reactionUIImage.color.b, 1);
        }

        // Start the fade coroutine
        fadeCoroutine = StartCoroutine(FadeTextAndImage());
    }

    private IEnumerator FadeTextAndImage()
    {
        yield return new WaitForSeconds(3f); // Wait before fading

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        Color originalTextColor = reactionUIText.color;
        Color originalImageColor = reactionUIImage != null ? reactionUIImage.color : Color.clear;

        // Gradually fade out the alpha
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0, elapsedTime / fadeDuration); // 0.5 ile 0 arasýnda fade

            // Update text alpha
            reactionUIText.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, alpha);

            // Update image alpha if it's assigned
            if (reactionUIImage != null)
            {
                reactionUIImage.color = new Color(originalImageColor.r, originalImageColor.g, originalImageColor.b, alpha);
            }

            yield return null;
        }

        // Ensure the text and image are fully transparent
        reactionUIText.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 0);

        if (reactionUIImage != null)
        {
            reactionUIImage.color = new Color(originalImageColor.r, originalImageColor.g, originalImageColor.b, 0);
        }

        fadeCoroutine = null;
    }
}
