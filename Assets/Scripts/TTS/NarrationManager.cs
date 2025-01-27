using System.Collections.Generic;
using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    [System.Serializable]
    public class EntityNarration
    {
        public string entityName; // Name of the entity. Could change to gameobject
        [TextArea] public string explanation; // Explanation to narrate.Could enclose in TTSInteractionHandler, but then no SSO
    }

    public List<EntityNarration> entityNarrations = new List<EntityNarration>();

    // Dictionary for quick lookup
    private Dictionary<string, string> narrationDictionary = new Dictionary<string, string>();

    private void Awake()
    {
        foreach (var entity in entityNarrations)
        {
            narrationDictionary[entity.entityName] = entity.explanation;
        }
    }

    public string GetExplanation(string entityName)
    {
        return narrationDictionary.ContainsKey(entityName) ? narrationDictionary[entityName] : null;
    }
}
