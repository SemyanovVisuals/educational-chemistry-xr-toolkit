using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ReactionUIManager reactionUIManager;

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
        ChemicalEntity.OnReactionTriggered += HandleReaction;   // Subscribe to the reaction event
    }

    private void OnDisable()
    {
        ChemicalEntity.OnReactionTriggered -= HandleReaction;   // Unsubscribe from the event to avoid memory leaks
    }

    private void HandleReaction(ChemicalEntity firstEntity, ChemicalEntity secondEntity)
    {
        if (firstEntity.formula == secondEntity.formula)
        {
            Destroy(secondEntity.gameObject);
            firstEntity.coefficient += secondEntity.coefficient;
            firstEntity.UpdateCoefficientUI();
        }
        else
        {
            var result = ChemicalReactionDatabase.GetProduct(firstEntity.formula, secondEntity.formula);

            if (result.product != "None" && firstEntity.coefficient >= result.coefficients.Item1 &&
                secondEntity.coefficient >= result.coefficients.Item2)
            {
                firstEntity.coefficient -= result.coefficients.Item1;
                secondEntity.coefficient -= result.coefficients.Item2;
                secondEntity.coefficient = secondEntity.coefficient;
                firstEntity.UpdateCoefficientUI();
                secondEntity.UpdateCoefficientUI();

                Vector3 newPosition = CalculateNewPosition(firstEntity.transform, secondEntity.transform, 0.3f);

                if (firstEntity.coefficient == 0 && secondEntity.coefficient == 0)
                {
                    // Reactants disappear
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

                // Products appear
                GameObject prefab = Resources.Load<GameObject>("Prefabs/" + result.product);

                string reactionText = "Combination Reaction:\n";
                reactionText += (result.coefficients.Item1 != 1 ? result.coefficients.Item1.ToString() : "") + firstEntity.formula + " + ";
                reactionText += (result.coefficients.Item2 != 1 ? result.coefficients.Item2.ToString() : "") + secondEntity.formula + " -> ";
                reactionText += (result.coefficients.Item3 != 1 ? result.coefficients.Item3.ToString() : "") + result.product;
                UpdateReactionUI(reactionText);

                if (prefab != null)
                {
                    GameObject product = Instantiate(prefab, newPosition, Quaternion.identity);
                    product.GetComponent<ChemicalEntity>().coefficient = result.coefficients.Item3;
                    product.GetComponent<ChemicalEntity>().UpdateCoefficientUI();
                }
            }
            else if (result.product != "None")
            {
                string reactionText = "To initiate a Combination Reaction:\n";
                reactionText += result.coefficients.Item1.ToString() + " x " + firstEntity.formula + "\n";
                reactionText += result.coefficients.Item2.ToString() + " x " + secondEntity.formula;
                UpdateReactionUI(reactionText);
            }
        }

        string product1 = ChemicalReactionDatabase.GetProduct(firstEntity.formula, secondEntity.formula).product;

        if (creatableCompounds.Contains(product1) && !unlockedCompounds.Contains(product1))
        {
            // Add the compound to the unlockedCompounds list
            unlockedCompounds.Add(product1);
            reactionUIManager.DisplayReactionText($"Compound unlocked: {product1}"); // Show unlocked compound

            // If all compounds are unlocked, show "Game Completed" message
            if (unlockedCompounds.Count == creatableCompounds.Count)
            {
                reactionUIManager.DisplayReactionText("Game Completed! All compounds have been created.");
            }
        }
    }

    private Vector3 CalculateNewPosition(Transform firstReactant, Transform secondReactant, float offsetDistance = 0.5f)
    {
        Vector3 midpoint = (firstReactant.position + secondReactant.position) / 2;
        Vector3 direction = (secondReactant.position - firstReactant.position).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 newPosition = midpoint + perpendicular * offsetDistance;

        // Ensure the new position is not inside any of the reactants
        Collider firstCollider = firstReactant.GetComponent<Collider>();
        Collider secondCollider = secondReactant.GetComponent<Collider>();

        if (firstCollider != null && firstCollider.bounds.Contains(newPosition))
        {
            newPosition += perpendicular * offsetDistance; // Further offset if intersecting
        }

        if (secondCollider != null && secondCollider.bounds.Contains(newPosition))
        {
            newPosition += perpendicular * offsetDistance; // Further offset if intersecting
        }

        return newPosition;
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

    private void RefreshInspectorLists()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
