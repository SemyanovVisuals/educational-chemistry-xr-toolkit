using Meta.WitAi.TTS.Utilities;
using UnityEngine;

public class TTSHandler : MonoBehaviour
{
    public TTSSpeaker ttsSpeaker; 
    public NarrationManager narrationManager;
    public float cooldownTime = 2f; // Cooldown time in seconds to prevent repeating

    private string lastEntitySpoken; 
    private float lastSpeakTime; 

    /// <summary>
    /// Speaks the explanation for the given entity (GameObject).
    /// </summary>
    /// <param name="entityObject">The GameObject being grabbed.</param>
    public void Speak(GameObject entityObject)
    {
        if (narrationManager == null || ttsSpeaker == null)
        {
            Debug.LogError("TTSHandler: NarrationManager or TTSSpeaker is not assigned.");
            return;
        }

        if (entityObject == null)
        {
            Debug.LogError("TTSHandler: Provided entityObject is null.");
            return;
        }

        // Get the name of the GameObject
        string entityName = ParseName(entityObject.name);

        // Prevent repeating the same entity's explanation if cooldown hasn't passed
        if (entityName == lastEntitySpoken && Time.time - lastSpeakTime < cooldownTime)
        {
            Debug.Log($"TTSHandler: Skipping speech for '{entityName}' due to cooldown.");
            return;
        }

        // If TTS is currently speaking, do not overlap
        if (ttsSpeaker.IsSpeaking)
        {
            Debug.Log("TTSHandler: Skipping speech because TTS is currently speaking.");
            return;
        }

        // Get the explanation for the entity
        string explanation = narrationManager.GetExplanation(entityName);

        if (!string.IsNullOrEmpty(explanation))
        {
            // Speak the explanation using TTSSpeaker
            ttsSpeaker.Speak(explanation);
            lastEntitySpoken = entityName; // Update the last entity spoken
            lastSpeakTime = Time.time; // Update the last speak time
        }
        else
        {
            // Handle missing explanation for the entity
            Debug.LogWarning($"TTSHandler: No explanation found for entity '{entityName}'.");
            ttsSpeaker.Speak($"I don't have any information about {entityName}.");
            lastEntitySpoken = entityName; // Still update the last entity spoken
            lastSpeakTime = Time.time; // Update the last speak time
        }
    }

    /// <summary>
    /// Stops the TTS playback for the given entity (GameObject).
    /// </summary>
    /// <param name="entityObject">The GameObject whose TTS should stop.</param>
    public void StopSpeaking(GameObject entityObject)
    {
        if (ttsSpeaker == null)
        {
            Debug.LogError("TTSHandler: TTSSpeaker is not assigned.");
            return;
        }

        if (entityObject == null)
        {
            Debug.LogError("TTSHandler: Provided entityObject is null.");
            return;
        }

        // Stop speaking the specific entity explanation
        ttsSpeaker.Stop(entityObject.name);
    }

    /// <summary>
    /// Stops all TTS playback.
    /// </summary>
    public void StopAll()
    {
        if (ttsSpeaker == null)
        {
            Debug.LogError("TTSHandler: TTSSpeaker is not assigned.");
            return;
        }

        // Stop all TTS playback
        ttsSpeaker.Stop();
    }

    /// <summary>
    /// Parses the original name of the GameObject to remove any parenthetical suffixes.
    /// </summary>
    /// <param name="originalName">The original name of the GameObject.</param>
    /// <returns>The parsed name without suffixes.</returns>
    private string ParseName(string originalName)
    {
        // For scene-created duplicates
        int index = originalName.IndexOf(" (");

        // For Unity clones like "(Clone)"
        if (index < 0)
        {
            index = originalName.IndexOf("(");
        }

        if (index >= 0)
        {
            return originalName.Substring(0, index).Trim(); // Trim to remove any extra spaces
        }

        // If no parentheses are found, return the original name
        return originalName;
    }
}
