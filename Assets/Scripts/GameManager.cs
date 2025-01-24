using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ReactionUIManager reactionUIManager;
    [SerializeField] private ReactionUIManager reactionGameManager;

    [Header("Collected Data")]
    [SerializeField] private List<string> availableElements = new List<string>(); // Names of elements available in the scene
    [SerializeField] private List<string> creatableCompounds = new List<string>(); // Names of compounds that can be created
    [SerializeField] private List<string> unlockedCompounds = new List<string>(); // Compounds unlocked by the user

    [Header("Full Item List")]
    [SerializeField] private List<string> unlockedEntities = new List<string>();

    private void Start()
    {
        // Collect the elements available in the scene
        CollectAvailableElements();

        // Add unique available elements to unlockedEntities
        AddAvailableElementsToUnlockedEntities();

        // Calculate the compounds that can be created
        CalculatePossibleCompounds();

        // Update the lists in the Inspector
        RefreshInspectorLists();
    }

    private void OnEnable()
    {
        ChemicalEntity.OnReactionTriggered += HandleReaction;   // Subscribe to the reaction event
    }

    private void OnDisable()
    {
        ChemicalEntity.OnReactionTriggered -= HandleReaction;   // Unsubscribe from the event to avoid memory leaks
    }

    // Note: HandleReaction is called twice for each pair or reactants but the repetition doesn't cause any issues.
    // TODO: [stretch goal] Think of how to avoid repetition of event trigger for the pair of reactants.
    // TODO: [stretch goal] Think of how to further optimize this function for better readability.
    private void HandleReaction(ChemicalEntity firstEntity, ChemicalEntity secondEntity)
    {
        // Combine identical entities by summing their coefficients
        if (firstEntity.formula == secondEntity.formula)
        {
            Destroy(secondEntity.gameObject);
            firstEntity.coefficient += secondEntity.coefficient;
            firstEntity.UpdateCoefficientUI();
        }
        else
        {
            // Get possible reactions for the two entities (currently, 1 reaction for each pair)
            var reactions = ChemicalReactionDatabase.GetProducts(firstEntity.formula, secondEntity.formula);
            
            // Reaction found in the database
            if (reactions != null && reactions.Count > 0)
            {
                var reaction = reactions[0];
                int numProducts = reaction.products.Count;
                string reactionText;
                
                // Check if coefficients are sufficient
                if (firstEntity.coefficient >= reaction.coefficients.Item1 &&
                    secondEntity.coefficient >= reaction.coefficients.Item2)
                {
                    // Deduct reactants' coefficients
                    firstEntity.coefficient -= reaction.coefficients.Item1;
                    secondEntity.coefficient -= reaction.coefficients.Item2;
                    firstEntity.UpdateCoefficientUI();
                    secondEntity.UpdateCoefficientUI();

                    // Determine new position for the product
                    Vector3 newPosition = CalculateOffsetPosition(firstEntity.transform.position, firstEntity?.gameObject);
                    newPosition = CalculateOffsetPosition(newPosition, secondEntity?.gameObject);

                    if (firstEntity.coefficient == 0 && secondEntity.coefficient == 0)
                    {
                        Destroy(firstEntity.gameObject);
                        Destroy(secondEntity.gameObject);
                        newPosition = firstEntity.transform.position;
                    }
                    else if (firstEntity.coefficient == 0)
                    {
                        Destroy(firstEntity.gameObject);
                    }
                    else if (secondEntity.coefficient == 0)
                    {
                        Destroy(secondEntity.gameObject);
                    }

                    // Instantiate each product
                    reactionText = (numProducts == 1) ? "Combination Reaction:\n" : "Replacement Reaction:\n";

                    if (numProducts == 1)   // Combination
                    {
                        var product = reaction.products.First();
                        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + product.Key);

                        // Update reaction UI text
                        reactionText += (reaction.coefficients.Item1 != 1 ? reaction.coefficients.Item1.ToString() : "") + 
                                        firstEntity.formula + " + ";
                        reactionText += (reaction.coefficients.Item2 != 1 ? reaction.coefficients.Item2.ToString() : "") + 
                                        secondEntity.formula + " -> ";
                        reactionText += (product.Value != 1 ? product.Value.ToString() : "") + product.Key;

                        if (prefab != null)
                        {
                            GameObject productInstance = Instantiate(prefab, newPosition, Quaternion.identity);
                            productInstance.GetComponent<ChemicalEntity>().coefficient = product.Value;
                            productInstance.GetComponent<ChemicalEntity>().UpdateCoefficientUI();
                        }
                    }
                    else if (numProducts == 2)   // Replacement
                    {
                        var product1 = reaction.products.First();
                        var product2 = reaction.products.Skip(1).FirstOrDefault();
                        GameObject prefab1 = Resources.Load<GameObject>("Prefabs/" + product1.Key);
                        GameObject prefab2 = Resources.Load<GameObject>("Prefabs/" + product2.Key);

                        // Update reaction UI text
                        reactionText += (reaction.coefficients.Item1 != 1 ? reaction.coefficients.Item1.ToString() : "") + 
                                        firstEntity.formula + " + ";
                        reactionText += (reaction.coefficients.Item2 != 1 ? reaction.coefficients.Item2.ToString() : "") + 
                                        secondEntity.formula + " -> ";
                        reactionText += (product1.Value != 1 ? product1.Value.ToString() : "") + product1.Key + " + ";
                        reactionText += (product2.Value != 1 ? product2.Value.ToString() : "") + product2.Key;

                        GameObject productInstance1 = Instantiate(prefab1, newPosition, Quaternion.identity);
                        productInstance1.GetComponent<ChemicalEntity>().coefficient = product1.Value;
                        productInstance1.GetComponent<ChemicalEntity>().UpdateCoefficientUI();
                        
                        // Ensure productInstance2 does not overlap with productInstance1 or the firstEntity/secondEntity (if still exist)
                        Vector3 newProductPosition = CalculateOffsetPosition(productInstance1.transform.position, firstEntity?.gameObject);
                        newProductPosition = CalculateOffsetPosition(newProductPosition, secondEntity?.gameObject);

                        GameObject productInstance2 = Instantiate(prefab2, newProductPosition, Quaternion.identity);
                        productInstance2.GetComponent<ChemicalEntity>().coefficient = product2.Value;
                        productInstance2.GetComponent<ChemicalEntity>().UpdateCoefficientUI();
                    }

                    UpdateReactionUI(reactionText);
                    
                    foreach (var product in reactions[0].products.Keys.ToList())
                    {
                        if (creatableCompounds.Contains(product) && !unlockedCompounds.Contains(product))
                        {
                            // Add the compound to the unlockedCompounds list
                            unlockedCompounds.Add(product);
                            unlockedEntities.Add(product); // Add to unlockedEntities as well
                            reactionGameManager.DisplayReactionText($"Unlocked Entities: {product}"); // Show unlocked compound
    
                            // If all compounds are unlocked, show "Game Completed" message
                            if (unlockedCompounds.Count == creatableCompounds.Count)
                            {
                                reactionGameManager.DisplayReactionText("Game Completed!");
                            }
                        }
                    }

                    
                    
                    return; // Stop after processing the reaction
                }
                
                // If no reaction could proceed due to insufficient coefficients, send a hint to the user
                reactionText = $"To initiate a {(numProducts == 1 ? "Combination" : "Replacement")} Reaction:\n";
                reactionText += reaction.coefficients.Item1.ToString() + " x " + firstEntity.formula + "\n";
                reactionText += reaction.coefficients.Item2.ToString() + " x " + secondEntity.formula;
                UpdateReactionUI(reactionText);
            }
            else
            {
                // No reaction found
                Debug.LogError("No valid reactions found for the given reactants.");
            }
        }
        
        
    }
    
    private Vector3 CalculateOffsetPosition(Vector3 referencePosition, GameObject entityToAvoid, float offsetDistance = 0.15f)
    {
        Vector3 offsetPosition = referencePosition + new Vector3(offsetDistance, 0, 0);  // Adjust the offset to avoid overlap
    
        if (entityToAvoid != null)
        {
            Collider entityCollider = entityToAvoid.GetComponent<Collider>();
            if (entityCollider != null && entityCollider.bounds.Contains(offsetPosition))
            {
                // Further adjust the offset if there's an overlap with the entity
                offsetPosition += new Vector3(offsetDistance, 0, 0);
            }
        }

        return offsetPosition;
    }
    
    private void UpdateReactionUI(string reactionText)
    {
        reactionUIManager.DisplayReactionText(reactionText);
    }

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

    private void AddAvailableElementsToUnlockedEntities()
    {
        HashSet<string> uniqueElements = new HashSet<string>(availableElements);
        unlockedEntities.AddRange(uniqueElements);
        Debug.Log($"Added {uniqueElements.Count} unique elements to Unlocked Entities.");
    }

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

                var reactionResult = ChemicalReactionDatabase.GetProducts(element1, element2);

                if (reactionResult != null)
                {
                    var products = reactionResult[0].products.Keys.ToList();

                    foreach (var product in products)
                    {
                        creatableCompounds.Add(product);
                        Debug.Log($"Possible compound: {product} (from {element1} and {element2})");
                    }
                }

                checkedPairs.Add(pairKey);
            }
        }

        Debug.Log($"Calculated {creatableCompounds.Count} possible compounds.");
    }

    private void RefreshInspectorLists()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
