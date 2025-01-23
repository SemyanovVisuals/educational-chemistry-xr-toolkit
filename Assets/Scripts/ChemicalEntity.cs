using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ChemicalEntity : MonoBehaviour
{
    public enum ChemEntity
    {
        Elements,
        Compounds
    }
    
    public enum Elements
    {
        None,
        H2,
        Fe,
        O2,
        N2,
    }

    public enum Compounds
    {
        None,
        H2O,
        NH3,
        CO2,
        HCN,
        CH4,
        NO,
        Fe2​O3
    }
    
    // Dictionary for Elements
    public static Dictionary<Elements, string> ElementsMap = new Dictionary<Elements, string>
    {
        { Elements.H2, "hydrogen gas" },
        { Elements.O2, "oxygen gas" },
        { Elements.N2, "nitrogen gas" },
        { Elements.Fe, "iron" },
        { Elements.None, "None" }
    };

    // Dictionary for Compounds
    public static Dictionary<Compounds, string> CompoundsMap = new Dictionary<Compounds, string>
    {
        { Compounds.H2O, "water" },
        { Compounds.NH3, "ammonia" },
        { Compounds.CO2, "carbon dioxide" },
        { Compounds.Fe2​O3, "iron oxide" },
        { Compounds.NO, "nitric oxide" },
        { Compounds.CH4, "methane" },
        { Compounds.HCN, "cyanide" },
        { Compounds.None, "None" }
    };

    // Serialize a defining info of the chemical entity
    [SerializeField] private ChemEntity entity;
    [SerializeField] private Elements element;
    [SerializeField] private Compounds compound;
    [SerializeField] private string formula;
    [SerializeField] private string name;
    [SerializeField] private int coefficient = 1;
    [SerializeField] private TextMeshProUGUI coefficientText;
    [SerializeField] private TextMeshProUGUI formulaNameText;
    [SerializeField] private ReactionUIManager reactionUIManager;
    
    private Grabbable grabbable;
    private GrabInteractable grabInteractable;
    private HandGrabInteractable handGrabInteractable;
    private bool isColliding = false;
    private GameObject collidingChemEntity;
    private bool isGrabbed = false;
    private Coroutine fadeCoroutine;
    
    private void Start()
    {
        if (entity == ChemEntity.Elements)
        {
            formula = element.ToString();
            name = ElementsMap[element];
            compound = Compounds.None;
        }
        else
        {
            formula = compound.ToString();
            name = CompoundsMap[compound];
            element = Elements.None;
        }

        UpdateCoefficientUI();
        formulaNameText.text = formula + "\n" + name;

        // grabbable = GetComponent<Grabbable>();
        grabInteractable = GetComponent<GrabInteractable>();
        handGrabInteractable = GetComponent<HandGrabInteractable>();

    }

    public void UpdateGrabbed(bool state)
    {
        isGrabbed = state;
    }

    private void UpdateCoefficientUI()
    {
        if (coefficient == 1)
        {
            coefficientText.text = "";
        }
        else
        {
            coefficientText.text = coefficient.ToString() + " x";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<ChemicalEntity>() != null)
        {
            isColliding = true;
            collidingChemEntity = other.transform.parent.gameObject;
            Debug.Log("CHEM COLLIDED");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<ChemicalEntity>() != null && collidingChemEntity == other.transform.parent.gameObject)
        {
            isColliding = false;
            collidingChemEntity = null;
        }
    }

    void Update()
    {
        if (isColliding && !isGrabbed && !collidingChemEntity.GetComponent<ChemicalEntity>().isGrabbed)
        {
            Reaction();
            isColliding = false; // Ensure this is reset to avoid repeated calls
            collidingChemEntity = null;
        }
        
    }

    private void Reaction()
    {
        ChemicalEntity second = collidingChemEntity.GetComponent<ChemicalEntity>(); 
        string secondFormula = second.formula;
        int secondCoefficient = second.coefficient;

        if (formula == secondFormula)
        {
            Destroy(collidingChemEntity);
            coefficient += secondCoefficient;
            UpdateCoefficientUI();
            // isColliding = false;
        }
        else
        {
            var result = ChemicalReactions.GetProduct(formula, secondFormula);

            if (result.product != "None" && coefficient >= result.coefficients.Item1 && 
                secondCoefficient >= result.coefficients.Item2)
            {
                
                coefficient -= result.coefficients.Item1;
                collidingChemEntity.GetComponent<ChemicalEntity>().coefficient -= result.coefficients.Item2;
                secondCoefficient = collidingChemEntity.GetComponent<ChemicalEntity>().coefficient;
                UpdateCoefficientUI();
                collidingChemEntity.GetComponent<ChemicalEntity>().UpdateCoefficientUI();
                
                Vector3 newPosition = CalculateNewPosition(transform, collidingChemEntity.transform, 0.3f);

                if (coefficient == 0 && secondCoefficient == 0)
                {
                    // Reactants disappear
                    Destroy(gameObject);
                    Destroy(collidingChemEntity);
                    newPosition = transform.position;
                }
                else if (coefficient == 0)
                {
                    Destroy(gameObject);
                }
                else if (secondCoefficient == 0)
                {
                    Destroy(collidingChemEntity);
                }
            
                // Products appear
                GameObject prefab = Resources.Load<GameObject>("Prefabs/" + result.product);

                string reactionText = "Combination Reaction:\n";
                reactionText += (result.coefficients.Item1 != 1 ? result.coefficients.Item1.ToString() : "") + formula + " + ";
                reactionText += (result.coefficients.Item2 != 1 ? result.coefficients.Item2.ToString() : "") + secondFormula + " -> ";
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
                reactionText += result.coefficients.Item1.ToString() + " x " + formula + "\n";
                reactionText += result.coefficients.Item2.ToString() + " x " + secondFormula;
                UpdateReactionUI(reactionText);
            }
        }
    }
    
    private Vector3 CalculateNewPosition(Transform firstReactant, Transform secondReactant, float offsetDistance = 0.5f)
    {
        // Step 1: Calculate the midpoint between the two reactants
        Vector3 midpoint = (firstReactant.position + secondReactant.position) / 2;

        // Step 2: Calculate the direction vector from the first to the second reactant
        Vector3 direction = (secondReactant.position - firstReactant.position).normalized;

        // Step 3: Find a perpendicular direction for the offset
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        // Step 4: Apply an offset along the perpendicular direction
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
