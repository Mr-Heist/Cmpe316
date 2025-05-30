using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Singleton instance for global access
    public static ScoreManager Instance;

    // Reference to the UI text element displaying the score
    public TextMeshProUGUI scoreText;

    // Internal score value
    private int score = 0;

    // Current score multiplier (default is 1)
    private int multiplier = 1;

    // Reference to the active multiplier coroutine (if any)
    private Coroutine multiplierRoutine;

    private void Awake()
    {
        // Set up the singleton instance
        Instance = this;
    }

    // Increases the score by the current multiplier and updates the UI
    public void AddPoint()
    {
        score += multiplier;
        scoreText.text = "Puan: " + score;
    }

    // Activates double point multiplier for a limited time
    public void ActivateMultiplier(float duration)
    {
        // If a multiplier is already active, stop it before starting a new one
        if (multiplierRoutine != null)
            StopCoroutine(multiplierRoutine);

        // Start a new multiplier coroutine
        multiplierRoutine = StartCoroutine(MultiplierRoutine(duration));
    }

    // Coroutine that enables double points for the given duration
    private System.Collections.IEnumerator MultiplierRoutine(float duration)
    {
        multiplier = 2;

        float timeLeft = duration;
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        multiplier = 1;
    }
}
