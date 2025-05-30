using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // The total number of lives the player starts with
    public int lives = 3;

    // Flag to track if the player is dead
    private bool isDead = false;

    // Flag to prevent the player from taking damage repeatedly in a short time
    private bool invulnerable = false;

    // Reference to the UI script that displays player health
    public PlayerHealthUI healthUI;

    // Called when the player takes damage
    public void TakeDamage()
    {
        // Ignore damage if the player is already dead or currently invulnerable
        if (isDead || invulnerable) return;

        // Decrease lives by 1
        lives--;
        UnityEngine.Debug.Log("Took damage! Remaining lives: " + lives);

        // Update the UI to reflect new life count
        if (healthUI != null)
        {
            healthUI.UpdateHealthDisplay(lives);
        }

        // Check if the player has no lives left
        if (lives <= 0)
        {
            isDead = true;
            GameOver();
        }
        else
        {
            // Start temporary invulnerability after taking damage
            StartCoroutine(TemporaryInvulnerability());
        }
    }

    // Coroutine that makes the player invulnerable for 1 second
    IEnumerator TemporaryInvulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(1f);
        invulnerable = false;
    }

    // Called when the player's lives reach zero
    void GameOver()
    {
        UnityEngine.Debug.Log("Game Over!");
        Time.timeScale = 0f; // Pause the game
    }

    // Increases the player's life by a given amount (up to max of 3)
    public void Heal(int amount)
    {
        // Cannot heal if the player is dead
        if (isDead) return;

        if (lives < 3)
        {
            lives += amount;
            if (lives > 3) lives = 3;

            // Update the UI after healing
            if (healthUI != null)
            {
                healthUI.UpdateHealthDisplay(lives);
            }

            UnityEngine.Debug.Log("Health restored! New lives: " + lives);
        }
        else
        {
            UnityEngine.Debug.Log("Already at full health!");
        }
    }
}
