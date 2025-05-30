using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    // References to the car prefabs
    public GameObject yellowCarPrefab;
    public GameObject redCarPrefab;
    public GameObject blueCarPrefab;

    // The location where the selected car will be spawned
    public Transform spawnPoint;

    // Dropdown UI for selecting car color
    public TMP_Dropdown colorDropdown;

    // Reference to the currently active car in the scene
    private GameObject currentCar;

    void Start()
    {
        // Add a listener to the dropdown to call ChangeCar() when the selection changes
        colorDropdown.onValueChanged.AddListener(delegate { ChangeCar(colorDropdown.value); });

        // Load the first car (Yellow) by default at start
        ChangeCar(0);
    }

    public void ChangeCar(int index)
    {
        // Destroy the previously selected car if it exists
        if (currentCar != null)
        {
            Destroy(currentCar);
        }

        // Instantiate a new car based on the selected dropdown index
        switch (index)
        {
            case 0:
                currentCar = Instantiate(yellowCarPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                break;
            case 1:
                currentCar = Instantiate(redCarPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                break;
            case 2:
                currentCar = Instantiate(blueCarPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                break;
        }
    }
}
