using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Log the name of the object that triggered the collider
        UnityEngine.Debug.Log("Collided with: " + other.name);

        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("Pickup collected!");

            // If ScoreManager exists, add a point
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoint();
            }

            // Destroy the pickup object after collection
            Destroy(gameObject);
        }
    }
}
