using UnityEngine;

public class SpeedBoostPickup : MonoBehaviour
{
    public float impulseAmount = 20f;
    public float duration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArabaKontrol kontrol = other.GetComponent<ArabaKontrol>();
            if (kontrol != null)
            {
                kontrol.ApplyImpulseBoost(impulseAmount, duration);
            }

            gameObject.SetActive(false);
        }
    }
}
