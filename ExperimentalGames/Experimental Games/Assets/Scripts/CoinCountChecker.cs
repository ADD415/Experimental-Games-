using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCountChecker : MonoBehaviour
{
    public int coinsNeeded = 10;
    public TextMeshProUGUI messageText; // Reference to a TextMeshProUGUI component for displaying messages


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerMovement = other.GetComponent<PlayerController>();

            if (playerMovement != null)
            {
                Debug.Log($"Player has {playerMovement.collectedCoins} coins.");

                if (playerMovement.collectedCoins >= coinsNeeded)
                {
                    DisplayMessage("Opened, you have enough coins!");
                    Destroy(gameObject); // Destroy the object this script is attached to
                }
                else
                {
                    DisplayMessage($"You need {coinsNeeded} coins to pass.");
                }
            }
            else
            {
                Debug.LogError("PlayerMovement component not found!");
            }
        }
    }

    // Helper method to display messages on the TextMeshProUGUI
    private void DisplayMessage(string message)
    {
        if (messageText != null)
        {
            // Update the TextMeshProUGUI text with the provided message
            messageText.text = message;

            // Use Invoke to clear the message after a short delay (e.g., 2 seconds)
            Invoke("ClearMessage", 2f);
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component reference not set in the inspector!");
        }
    }

    // Helper method to clear the TextMeshProUGUI message
    private void ClearMessage()
    {
        if (messageText != null)
        {
            // Clear the TextMeshProUGUI text
            messageText.text = "";
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component reference not set in the inspector!");
        }
    }
}
