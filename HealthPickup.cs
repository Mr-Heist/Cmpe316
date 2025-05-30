using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // The amount of health this pickup restores
    public int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Try to get the PlayerHealth component from the player
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // Heal the player by the specified amount
                player.Heal(healAmount);
            }

            // Disable the pickup object so it disappears from the scene
            gameObject.SetActive(false);
        }
    }
}
