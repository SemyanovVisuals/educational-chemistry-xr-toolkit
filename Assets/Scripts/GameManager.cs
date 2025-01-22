using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ReactionUIManager reactionUIManager;
    
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
}
