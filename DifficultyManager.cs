using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // Static reference for Singleton pattern
    public static DifficultyManager Instance;

    // Enum representing available difficulty levels
    public enum DifficultyLevel { Easy, Medium, Hard }

    // Current selected difficulty level (default is Easy)
    public DifficultyLevel CurrentDifficulty = DifficultyLevel.Easy;

    void Awake()
    {
        // Singleton setup: if no instance exists, assign this one
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }
}
