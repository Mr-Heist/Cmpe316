using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int lives = 3;
    private bool isDead = false;
    private bool invulnerable = false;

    public PlayerHealthUI healthUI; // ❤️ UI script bağlantısı

    public void TakeDamage()
    {
        if (isDead || invulnerable) return;

        lives--;
        UnityEngine.Debug.Log("Hasar alındı! Kalan can: " + lives);

        // UI'da kalp sayısını güncelle
        if (healthUI != null)
        {
            healthUI.UpdateHealthDisplay(lives);
        }

        if (lives <= 0)
        {
            isDead = true;
            GameOver();
        }
        else
        {
            StartCoroutine(TemporaryInvulnerability());
        }
    }

    IEnumerator TemporaryInvulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(1f);
        invulnerable = false;
    }

    void GameOver()
    {
        UnityEngine.Debug.Log("Game Over!");
        Time.timeScale = 0f;
    }
    public void Heal(int amount)
    {
        if (isDead) return;

        if (lives < 3)
        {
            lives += amount;
            if (lives > 3) lives = 3;

            if (healthUI != null)
            {
                healthUI.UpdateHealthDisplay(lives);
            }

            UnityEngine.Debug.Log("Can toplandı! Yeni can: " + lives);
        }
        else
        {
            UnityEngine.Debug.Log("Zaten tam can!");
        }
    }

}
