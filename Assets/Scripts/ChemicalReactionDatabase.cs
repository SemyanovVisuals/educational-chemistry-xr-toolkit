using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ChemicalReactionDatabase : MonoBehaviour
{
    // The Map to store chemical reactions
    public static Dictionary<string, Dictionary<string, Dictionary<string, (int, int, int)>>> Map =
        new Dictionary<string, Dictionary<string, Dictionary<string, (int, int, int)>>>();

    // Path to the text file (make sure this file exists in your Assets/Resources folder)
    public TextAsset reactionsFile;

    void Start()
    {
        // Read all lines from the file
        string[] lines = reactionsFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // Parse each reaction and fill the Map
        foreach (string line in lines)
        {
            try
            {
                ParseAndAddReaction(line.Trim());
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing line: '{line}' - {ex.Message}");
            }
        }

        // Log success
        Debug.Log("Map populated successfully!");
        Debug.Log(MapToString());
    }

    // Parse a single reaction and add it to the Map
    private void ParseAndAddReaction(string reaction)
    {
        // Split the reaction into left-hand side (LHS) and right-hand side (RHS)
        string[] parts = reaction.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            throw new FormatException("Reaction must contain exactly one '->' symbol.");

        string[] lhs = parts[0].Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
        string rhs = parts[1].Trim();

        // Parse the RHS (e.g., "2 H2O")
        (int productCoefficient, string product) = ParseCompound(rhs);

        // Parse the LHS components (e.g., "2 H2", "O2")
        var lhsComponents = new List<(int coefficient, string compound)>();
        foreach (string component in lhs)
        {
            lhsComponents.Add(ParseCompound(component.Trim()));
        }

        // Add the reaction to the Map for each LHS compound
        for (int i = 0; i < lhsComponents.Count; i++)
        {
            var lhsCompound = lhsComponents[i];

            for (int j = 0; j < lhsComponents.Count; j++)
            {
                if (i == j) continue; // Avoid adding reactions to itself

                var otherLhsCompound = lhsComponents[j];
                AddToMap(lhsCompound.compound, otherLhsCompound.compound, product, lhsCompound.coefficient, otherLhsCompound.coefficient, productCoefficient);
            }
        }
    }

    // Parse a compound string like "2 H2" or "O2"
    private (int coefficient, string compound) ParseCompound(string compoundString)
    {
        string[] parts = compoundString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
            return (1, parts[0]); // If no coefficient is given, assume it's 1

        if (parts.Length == 2 && int.TryParse(parts[0], out int coefficient))
            return (coefficient, parts[1]);

        throw new FormatException($"Invalid compound format: '{compoundString}'");
    }

    // Add a reaction to the Map
    private void AddToMap(string reactant1, string reactant2, string product, int coefficient1, int coefficient2, int productCoefficient)
    {
        if (!Map.ContainsKey(reactant1))
        {
            Map[reactant1] = new Dictionary<string, Dictionary<string, (int, int, int)>>();
        }

        if (!Map[reactant1].ContainsKey(reactant2))
        {
            Map[reactant1][reactant2] = new Dictionary<string, (int, int, int)>();
        }

        Map[reactant1][reactant2][product] = (coefficient1, coefficient2, productCoefficient);
    }

    // Convert the Map to a readable string for debugging
    private string MapToString()
    {
        var result = new System.Text.StringBuilder();
        foreach (var reactant1 in Map)
        {
            result.AppendLine($"Reactant1: {reactant1.Key}");
            foreach (var reactant2 in reactant1.Value)
            {
                result.AppendLine($"\tReactant2: {reactant2.Key}");
                foreach (var product in reactant2.Value)
                {
                    var coefficients = product.Value;
                    result.AppendLine($"\t\tProduct: {product.Key} ({coefficients.Item1}, {coefficients.Item2}, {coefficients.Item3})");
                }
            }
        }
        return result.ToString();
    }
    

    public static (string product, (int, int, int) coefficients) GetProduct(string reactant1, string reactant2)
    {
        // Check the first reactant and its associated second reactant
        if (Map.ContainsKey(reactant1) && Map[reactant1].ContainsKey(reactant2))
        {
            var product = Map[reactant1][reactant2];
            // Get the product and the last coefficient
            var productKey = product.Keys.First();
            var coefficients = product[productKey];
            
            return (productKey, coefficients);
        }

        // If no reaction found
        Debug.LogError("Reaction not found!");
        return ("None", (0, 0, 0));  // Return default value
    }
}
