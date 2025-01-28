using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ReactionUIManager reactionUIManager;
    //[SerializeField] private ReactionUIManager reactionGameManager;
    [SerializeField] private NotificationBanner banner;

    [Header("Entities Lists")]
    [SerializeField] private List<string> allEntities = new List<string>(); // Names of all entities supported by app 
    [SerializeField] private List<string> unlockedEntities = new List<string>();

    private void Start()
    {
        // Load Entities Lists
        PopulateAllEntities();
        PopulateUnlockedEntities();
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
                    
                    // Update unlocked entities list
                    foreach (var product in reactions[0].products.Keys.ToList())
                    {
                        if (!unlockedEntities.Contains(product))
                        {
                            unlockedEntities.Add(product);
                            //reactionGameManager.DisplayReactionText("New Entity Unlocked!");
                            banner.SetNotificationText($"New Entity Unlocked:\n{product}");
                            banner.gameObject.SetActive(true);
                        }
                    }
                    
                    // Check if the game is completed
                    if (allEntities.Count == unlockedEntities.Count)
                    {
                        StartCoroutine(DisplayGameCompletedWithDelay(2.5f));
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
                // No reaction found in the database
                Debug.Log($"No reaction between {firstEntity.formula} and {secondEntity.formula}.");
            }
        }
    }
    private  IEnumerator DisplayGameCompletedWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        reactionUIManager.DisplayReactionText("Game Completed!");
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
    
    // Load all supported entities from database
    private void PopulateAllEntities()
    {
        allEntities = ChemicalReactionDatabase.GetAllEntities();
        Debug.Log("All Entities Supported: " + string.Join(", ", allEntities));
    }
    
    // Hard-coded starting entities
    private void PopulateUnlockedEntities()
    {
        unlockedEntities.Add("H2");
        unlockedEntities.Add("O2");
        unlockedEntities.Add("N2");
        unlockedEntities.Add("C");
        unlockedEntities.Add("Fe");
    }
}
