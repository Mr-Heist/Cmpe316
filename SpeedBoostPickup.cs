using UnityEngine;

public class SpeedBoostPickup : MonoBehaviour
{
    // Amount of impulse force to apply to the player
    public float impulseAmount = 20f;

    // Duration the boost effect will last
    public float duration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Try to get the ArabaKontrol script from the player
            ArabaKontrol kontrol = other.GetComponent<ArabaKontrol>();
            if (kontrol != null)
            {
                // Apply a temporary speed boost to the car
                kontrol.ApplyImpulseBoost(impulseAmount, duration);
            }

            // Disable the pickup so it's no longer visible or interactable
            gameObject.SetActive(false);
        }
    }
}
