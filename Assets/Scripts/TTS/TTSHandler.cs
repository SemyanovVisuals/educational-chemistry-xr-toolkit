using System.Linq;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;

public class TTSHandler : MonoBehaviour
{
    public TTSSpeaker ttsSpeaker; 
    public NarrationManager narrationManager;
    public float cooldownTime = 60f; // Cooldown time in seconds to prevent repeating
    

    private string lastEntitySpoken; 
    private float lastSpeakTime; 

    /// <summary>
    /// Speaks the explanation for the given entity (GameObject).
    /// </summary>
    /// <param name="entityObject">The GameObject being grabbed.</param>
    public void Speak(string entityFormula, string entityName)
    {
        if (narrationManager == null || ttsSpeaker == null)
        {
            Debug.LogError("TTSHandler: NarrationManager or TTSSpeaker is not assigned.");
            return;
        }

        if (entityFormula == null)
        {
            Debug.LogError("TTSHandler: Provided entityObject is null.");
            return;
        }
        
        // Get the name of the GameObject
        //string entityName = ParseName(entityObject.name);
        // string entityFormula = entity.formula;
        // string entityName = entity.entityName;

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
        // string explanation = narrationManager.GetExplanation(entityName);
        var explanation = TextToVoice(entityFormula, entityName);

        if (!string.IsNullOrEmpty(explanation) && entityName != lastEntitySpoken)
        {
            // Speak the explanation using TTSSpeaker
            ttsSpeaker.Speak(explanation);
            lastEntitySpoken = entityName; // Update the last entity spoken
            lastSpeakTime = Time.time; // Update the last speak time
        }
        else if (string.IsNullOrEmpty(explanation))
        {
            // Handle missing explanation for the entity
            Debug.LogWarning($"TTSHandler: No explanation found for entity '{entityName}'.");
            ttsSpeaker.Speak($"I don't have any information about {entityName}.");
            lastEntitySpoken = entityName; // Still update the last entity spoken
            lastSpeakTime = Time.time; // Update the last speak time
        }
    }

    private static string TextToVoice(string entityFormula, string entityName)
    {
        // Collect all fellow reactants
        var fellowReactants = ChemicalReactionDatabase.GetFellowReactants(entityFormula);
        if (fellowReactants == null || fellowReactants.Count == 0) return entityName;

        var text = entityName + "\nReacts with ";

        // Check if there's more than one fellow reactant
        if (fellowReactants.Count > 1)
        {
            for (int i = 0; i < fellowReactants.Count; i++)
            {
                var name = ChemicalReactionDatabase.GetEntityName(fellowReactants[i]);
                if (name == null) continue;  // Skip if null

                // Add a comma before all but the last element
                if (i < fellowReactants.Count - 1)
                {
                    text += name + ", ";
                }
                else
                {
                    // Add "and" before the last element
                    text += "and " + name;
                }
            }
        }
        else
        {
            // If only one fellow reactant, just add its name
            var name = ChemicalReactionDatabase.GetEntityName(fellowReactants[0]);
            if (name != null)
            {
                text += name;
            }
        }

        return text;
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
