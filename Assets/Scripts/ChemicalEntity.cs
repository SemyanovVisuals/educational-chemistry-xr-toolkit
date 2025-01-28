using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Update = UnityEngine.PlayerLoop.Update;

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
        C,
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
        Fe2O3
    }
    
    // Dictionary for Basic Entities
    public static Dictionary<Elements, string> ElementsMap = new Dictionary<Elements, string>
    {
        { Elements.H2, "hydrogen gas" },
        { Elements.O2, "oxygen gas" },
        { Elements.N2, "nitrogen gas" },
        { Elements.Fe, "iron" },
        { Elements.C, "carbon" },
        { Elements.None, "None" }
    };

    // Dictionary for Compounds
    public static Dictionary<Compounds, string> CompoundsMap = new Dictionary<Compounds, string>
    {
        { Compounds.H2O, "water" },
        { Compounds.NH3, "ammonia" },
        { Compounds.CO2, "carbon dioxide" },
        { Compounds.Fe2O3, "iron oxide" },
        { Compounds.NO, "nitric oxide" },
        { Compounds.CH4, "methane" },
        { Compounds.HCN, "cyanide" },
        { Compounds.None, "None" }
    };

    // Serialize a defining info of the chemical entity
    [SerializeField] public ChemEntity entity;
    [SerializeField] public Elements element;
    [SerializeField] private Compounds compound;
    [SerializeField] public string formula;
    [SerializeField] private string name;
    [SerializeField] public int coefficient = 1;
    [SerializeField] private TextMeshProUGUI coefficientText;
    [SerializeField] private TextMeshProUGUI formulaNameText;
    [SerializeField] GameObject user;
    // [SerializeField] private ReactionUIManager reactionUIManager;
    
    private Grabbable grabbable;
    private GrabInteractable grabInteractable;
    private HandGrabInteractable handGrabInteractable;
    private bool isColliding = false;
    private GameObject collidingChemEntity;
    private bool isGrabbed = false;
    private Coroutine fadeCoroutine;
    public static event Action<ChemicalEntity, ChemicalEntity> OnReactionTriggered;
    private bool hasTriggeredReaction = false;
    
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

        grabInteractable = GetComponent<GrabInteractable>();
        handGrabInteractable = GetComponent<HandGrabInteractable>();

        user = FindAnyObjectByType<User>().gameObject;
    }
    
    void Update()
    {
        if (isColliding && !isGrabbed)
        {
            if (collidingChemEntity != null && !collidingChemEntity.GetComponent<ChemicalEntity>().isGrabbed)
            {
                OnReactionTriggered?.Invoke(this, collidingChemEntity.GetComponent<ChemicalEntity>());
                
                isColliding = false; // Ensure this is reset to avoid repeated calls
                collidingChemEntity = null;
            }
        }

        if (user != null)
        {
            // Make the text look at the user
            formulaNameText.transform.LookAt(2 * formulaNameText.transform.position - user.transform.position);
            coefficientText.transform.LookAt(2 * formulaNameText.transform.position - user.transform.position);
        }
    }


    public void UpdateGrabbed(bool state)
    {
        isGrabbed = state;
    }

    public void UpdateCoefficientUI()
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
            collidingChemEntity = other.GetComponentInParent<ChemicalEntity>().gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<ChemicalEntity>() != null && 
            collidingChemEntity == other.GetComponentInParent<ChemicalEntity>().gameObject)
        {
            isColliding = false;
            collidingChemEntity = null;
        }
    }
}
