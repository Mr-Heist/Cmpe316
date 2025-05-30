using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    // Reference to the dropdown UI for selecting difficulty
    public TMP_Dropdown difficultyDropdown;

    public void StartGame()
    {
        // Get the selected difficulty from the dropdown and convert it to lowercase
        string selectedDifficulty = difficultyDropdown.options[difficultyDropdown.value].text.ToLower();

        // Save the selected difficulty in PlayerPrefs to be used in the game
        PlayerPrefs.SetString("difficulty", selectedDifficulty);

        // Load the main game scene
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToGarage()
    {
        // Load the scene that represents the garage screen
        SceneManager.LoadScene("GarageScene");
    }

    public void GoToLeaderboard()
    {
        // Load the scene that displays the leaderboard
        SceneManager.LoadScene("LeaderboardScene");
    }
    public void GoToMainManu()
    {
        // Load the scene that displays the leaderboard
        SceneManager.LoadScene("MainManu");
    }
}
