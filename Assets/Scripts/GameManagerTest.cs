using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    [Header("Collected Data")]
    [SerializeField] private List<string> availableElements = new List<string>(); // Names of elements available in the scene
    [SerializeField] private List<string> creatableCompounds = new List<string>(); // Names of compounds that can be created
    [SerializeField] private List<string> unlockedCompounds = new List<string>(); // Compounds unlocked by the user

    private void Start()
    {
        // Collect the elements available in the scene
        CollectAvailableElements();

        // Calculate the compounds that can be created
        CalculatePossibleCompounds();

        // Update the lists in the Inspector
        RefreshInspectorLists();
    }

    private void OnEnable()
    {
        ChemicalEntity.OnReactionTriggered += HandleReaction; // Subscribe to the reaction event
    }

    private void OnDisable()
    {
        ChemicalEntity.OnReactionTriggered -= HandleReaction; // Unsubscribe from the reaction event
    }

    /// <summary>
    /// Collects the available elements in the scene and records their names.
    /// </summary>
    private void CollectAvailableElements()
    {
        ChemicalEntity[] allEntities = FindObjectsOfType<ChemicalEntity>();

        foreach (var entity in allEntities)
        {
            if (entity.entity == ChemicalEntity.ChemEntity.Elements)
            {
                // Add the name of the element from the enum
                string elementName = entity.element.ToString();
                availableElements.Add(elementName);
                Debug.Log($"Collected element: {elementName}");
            }
        }

        Debug.Log($"Collected {availableElements.Count} available elements in the scene.");
    }

    /// <summary>
    /// Calculates the compounds that can be created based on the available elements.
    /// </summary>
    private void CalculatePossibleCompounds()
    {
        HashSet<string> checkedPairs = new HashSet<string>();

        foreach (var element1 in availableElements)
        {
            foreach (var element2 in availableElements)
            {
                if (element1 == element2) continue;

                string pairKey = $"{element1}-{element2}";
                string reversePairKey = $"{element2}-{element1}";

                if (checkedPairs.Contains(pairKey) || checkedPairs.Contains(reversePairKey))
                    continue;

                Debug.Log($"Checking reaction for: {element1} and {element2}");

                var reactionResult = ChemicalReactionDatabase.GetProduct(element1, element2);

                if (reactionResult.product != "None")
                {
                    creatableCompounds.Add(reactionResult.product);
                    Debug.Log($"Possible compound: {reactionResult.product} (from {element1} and {element2})");
                }

                checkedPairs.Add(pairKey);
            }
        }

        Debug.Log($"Calculated {creatableCompounds.Count} possible compounds.");
    }

    /// <summary>
    /// Executes when a reaction is triggered.
    /// </summary>
    private void HandleReaction(ChemicalEntity firstEntity, ChemicalEntity secondEntity)
    {
        string product = ChemicalReactionDatabase.GetProduct(firstEntity.formula, secondEntity.formula).product;

        if (creatableCompounds.Contains(product) && !unlockedCompounds.Contains(product))
        {
            // Add the compound to the unlockedCompounds list
            unlockedCompounds.Add(product);
            Debug.Log($"Compound unlocked: {product}");

            // If all compounds are unlocked, show "Game Completed" message
            if (unlockedCompounds.Count == creatableCompounds.Count)
            {
                Debug.Log("Game Completed! All compounds have been created.");
            }
        }
    }

    /// <summary>
    /// Updates the lists in the Unity Inspector.
    /// </summary>
    private void RefreshInspectorLists()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
