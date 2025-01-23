using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChemicalReactionDatabase
{
    public static Dictionary<string, Dictionary<string, Dictionary<string, (int, int, int)>>> Map = new Dictionary<string, Dictionary<string, Dictionary<string, (int, int, int)>>>
{
    // H2
    {
        "H2", new Dictionary<string, Dictionary<string, (int, int, int)>>()
        {
            {
                "O2", new Dictionary<string, (int, int, int)>()
                {
                    { "H2O", (2, 1, 2) }    // Reaction: 2H2 + O2 → 2H2O
                }
            },

            {
                "N2", new Dictionary<string, (int, int, int)>()
                {
                    { "NH3", (3, 1, 2) }    // Reaction: 3H2 + N2 → 2NH3
                }
            },

            {
                "C", new Dictionary<string, (int, int, int)>()
                {
                    { "CH4", (4, 1, 1) }    // Reaction: 4H2 + C → CH4
                }
            }
        }
    },
    
    // O2
    {
        "O2", new Dictionary<string, Dictionary<string, (int, int, int)>>()
        {
            {
                "H2", new Dictionary<string, (int, int, int)>()
                {
                    { "H2O", (1, 2, 2) }    // Reaction: O2 + 2H2 → 2H2O
                }
            }
        }
    },
    
    // N2
    {
        "N2", new Dictionary<string, Dictionary<string, (int, int, int)>>()
        {
            {
                "H2", new Dictionary<string, (int, int, int)>()
                {
                    { "NH3", (1, 3, 2) }    // Reaction: N2 + 3H2 → 2NH3
                }
            }
        }
    },

    // C (Carbon)
    {
        "C", new Dictionary<string, Dictionary<string, (int, int, int)>>()
        {
            {
                "H2", new Dictionary<string, (int, int, int)>()
                {
                    { "CH4", (1, 4, 1) }    // Reaction: C + 4H2 → CH4
                }
            }
        }
    }
};


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
