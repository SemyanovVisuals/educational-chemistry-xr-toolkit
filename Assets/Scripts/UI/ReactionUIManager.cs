using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ReactionUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reactionUIText;
    [SerializeField] private Image reactionUIImage;
    [SerializeField] private AudioSource audioSource;
    
    private Coroutine fadeCoroutine;
    private string introText = "Welcome to the game!\n\n" +
                                "Your goal: unlock all compounds from the catalog.\n\n" +
                                "Start by: initiating reactions between " +
                                "the available chemical entities!";
    private void Start()
    {
        DisplayReactionText(introText, 10, false);
    }

    public void DisplayReactionText(string reactionText, int fontSize = 13, bool playSound = true)
    {
        reactionUIText.fontSize = fontSize;
        reactionUIText.text = reactionText;
        if (playSound)
        {
            audioSource.Play();
        }
    }
}
