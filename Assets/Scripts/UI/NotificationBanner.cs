using TMPro;
using UnityEngine;
using UnityEngine.UI; // For Image and Text components

public class NotificationBanner : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI textMP;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float displayDuration = 3.0f;
    [SerializeField] private Graphic[] graphics;

    void Awake()
    {
        // Get references to the AudioSource and all Graphic components
        audioSource = GetComponent<AudioSource>();
        graphics = GetComponentsInChildren<Graphic>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing!");
        }

        if (graphics.Length == 0)
        {
            Debug.LogError("No Image/Text components found to fade!");
        }
    }

    void OnEnable()
    {
        // Reset alpha for all graphics and start the notification behavior
        foreach (var graphic in graphics)
        {
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 1.0f); // Fully visible
        }

        // Play the sound
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Start the fade-out coroutine
        StartCoroutine(FadeOutAndDisable());
    }

    private System.Collections.IEnumerator FadeOutAndDisable()
    {
        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Gradually reduce the alpha of each graphic
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            foreach (var graphic in graphics)
            {
                Color color = graphic.color;
                float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeDuration);
                graphic.color = new Color(color.r, color.g, color.b, alpha);
            }

            yield return null;
        }

        // Ensure all graphics are fully transparent
        foreach (var graphic in graphics)
        {
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 0.0f); // Fully transparent
        }

        // Disable the game object
        gameObject.SetActive(false);
    }

    public void SetNotificationText(string text)
    {
        textMP.text = text;
    }
}
