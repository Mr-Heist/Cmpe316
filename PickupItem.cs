using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("Bir şeye çarptım: " + other.name);

        if (other.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("Pickup alındı!");

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoint();
            }

            Destroy(gameObject);
        }
    }
}
