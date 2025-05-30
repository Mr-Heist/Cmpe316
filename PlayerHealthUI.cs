using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    // Array of heart icons representing the player's health
    public Image[] hearts;

    // Updates the heart display based on the current number of lives
    public void UpdateHealthDisplay(int currentLives)
    {
        // Enable hearts that are within the current life count, disable the rest
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentLives;
        }
    }
}
