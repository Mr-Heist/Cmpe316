using UnityEngine;

public class DoublePointsPickup : MonoBehaviour
{
    // Duration for which the double points effect is active
    public float duration = 15f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Activate the score multiplier effect for the specified duration
            ScoreManager.Instance.ActivateMultiplier(duration);

            // Disable the pickup object so it disappears from the scene
            gameObject.SetActive(false);
        }
    }
}
