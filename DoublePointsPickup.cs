using UnityEngine;

public class DoublePointsPickup : MonoBehaviour
{
    public float duration = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.Instance.ActivateMultiplier(duration);
            gameObject.SetActive(false); // item kaybolsun
        }
    }
}
