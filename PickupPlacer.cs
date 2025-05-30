using UnityEngine;

public class PickupPlacer : MonoBehaviour
{
    public GameObject pickupPrefab;
    public float spacing = 5f;
    public float yOffset = 1f;

    [ContextMenu("Yollara Pickup Dağıt")]
    void PlacePickups()
    {
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");

        foreach (GameObject road in roads)
        {
            Renderer rend = road.GetComponent<Renderer>();
            if (rend == null) continue;

            Vector3 min = rend.bounds.min;
            Vector3 max = rend.bounds.max;

            float length = rend.bounds.size.z;
            int count = Mathf.FloorToInt(length / spacing);

            for (int i = 0; i <= count; i++)
            {
                float t = (float)i / count;
                float z = Mathf.Lerp(min.z, max.z, t);

                // DÜZENLİ ORTADAN HİZALI DİZİLİM 👇
                float x = rend.bounds.center.x;
                float y = road.transform.position.y + yOffset;

                Vector3 point = new Vector3(x, y, z);

                GameObject pickup = Instantiate(pickupPrefab, point, Quaternion.identity);
                pickup.transform.SetParent(this.transform);
            }
        }

        UnityEngine.Debug.Log("Pickup'lar ortadan dizildi.");
    }
}
