using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class ReactionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reactionText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void UpdateUI(string newText)
    {
        reactionText.text = newText;
        Debug.Log("HEEEEEERE  " + newText);
    }
}
