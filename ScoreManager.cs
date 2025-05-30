using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreText;
    private int score = 0;

    private int multiplier = 1; // ✅ Yeni: çarpan

    private void Awake()
    {
        Instance = this;
    }

    public void AddPoint()
    {
        score += multiplier;
        scoreText.text = "Puan: " + score;
    }

    public void ActivateMultiplier(float duration)
    {
        StopAllCoroutines(); // Aynı anda birden fazla olmasın
        StartCoroutine(MultiplierRoutine(duration));
    }

    private System.Collections.IEnumerator MultiplierRoutine(float duration)
    {
        multiplier = 2;
        Debug.Log("2x puan aktif!");
        yield return new WaitForSeconds(duration);
        multiplier = 1;
        Debug.Log("2x puan bitti.");
    }
}
