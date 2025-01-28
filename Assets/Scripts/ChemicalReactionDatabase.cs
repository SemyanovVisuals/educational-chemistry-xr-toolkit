using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ChemicalReactionDatabase : MonoBehaviour
{
    // The Map to store chemical reactions
    public static Dictionary<string, Dictionary<string, List<(Dictionary<string, int> products, (int, int) coefficients)>>> Map =
        new Dictionary<string, Dictionary<string, List<(Dictionary<string, int> products, (int, int) coefficients)>>>();

    // Path to the text file (make sure this file exists in your Assets/Resources folder)
    public TextAsset reactionsFile;

    void Awake()
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
        string[] rhs = parts[1].Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

        // Parse the RHS (products)
        var products = new Dictionary<string, int>();
        foreach (string productString in rhs)
        {
            var (productCoefficient, product) = ParseCompound(productString.Trim());
            products[product] = productCoefficient;
        }

        // Parse the LHS components (reactants)
        var lhsComponents = new List<(int coefficient, string compound)>();
        foreach (string component in lhs)
        {
            lhsComponents.Add(ParseCompound(component.Trim()));
        }

        // Add the reaction to the Map for each pair of LHS compounds
        for (int i = 0; i < lhsComponents.Count; i++)
        {
            var lhsCompound1 = lhsComponents[i];
            for (int j = 0; j < lhsComponents.Count; j++)
            {
                if (i == j) continue; // Avoid adding reactions between the same compound

                var lhsCompound2 = lhsComponents[j];
                AddToMap(lhsCompound1.compound, lhsCompound2.compound, products, lhsCompound1.coefficient, lhsCompound2.coefficient);
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
    private void AddToMap(string reactant1, string reactant2, Dictionary<string, int> products, int coefficient1, int coefficient2)
    {
        if (!Map.ContainsKey(reactant1))
        {
            Map[reactant1] = new Dictionary<string, List<(Dictionary<string, int> products, (int, int) coefficients)>>();
        }

        if (!Map[reactant1].ContainsKey(reactant2))
        {
            Map[reactant1][reactant2] = new List<(Dictionary<string, int> products, (int, int) coefficients)>();
        }

        Map[reactant1][reactant2].Add((products, (coefficient1, coefficient2)));
    }

    // Convert the Map to a readable string for debugging
    private string MapToString()
    {
        var result = new System.Text.StringBuilder();
        result.AppendLine("Map/database:");
        foreach (var reactant1 in Map)
        {
            result.AppendLine($"Reactant1: {reactant1.Key}");
            foreach (var reactant2 in reactant1.Value)
            {
                result.AppendLine($"\tReactant2: {reactant2.Key}");
                foreach (var reaction in reactant2.Value)
                {
                    var coefficients = reaction.coefficients;
                    var products = string.Join(", ", reaction.products.Select(p => $"{p.Value} {p.Key}"));
                    result.AppendLine($"\t\tProducts: {products} (Reactants Coefficients: {coefficients.Item1}, {coefficients.Item2})");
                }
            }
        }
        return result.ToString();
    }

    // Retrieve the list of possible products + coefficients for a pair of reactants
    public static List<(Dictionary<string, int> products, (int, int) coefficients)> GetProducts(string reactant1, string reactant2)
    {
        if (Map.ContainsKey(reactant1) && Map[reactant1].ContainsKey(reactant2))
        {
            return Map[reactant1][reactant2];
        }

        return null;
    }

    // Retrieve the list of all entities supported by the database (reactants and/or products) for debugging
    public static List<string> GetAllEntities()
    {
        var entities = new List<string>();
        
        foreach (var reactant1 in Map.Keys)
        {
            if (!entities.Contains(reactant1))
            {
                entities.Add(reactant1);
            }

            foreach (var reactant2 in Map[reactant1].Keys)
            {
                if (!entities.Contains(reactant2))
                {
                    entities.Add(reactant2);
                }

                foreach (var product in Map[reactant1][reactant2].First().products.Keys)
                {
                    if (!entities.Contains(product))
                    {
                        entities.Add(product);
                    }
                }
            }
        }

        return entities;
    }
}