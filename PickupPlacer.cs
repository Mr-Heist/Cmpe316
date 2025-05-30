using UnityEngine;

public class PickupPlacer : MonoBehaviour
{
    // The prefab to be placed on roads
    public GameObject pickupPrefab;

    // Spacing between pickups along the road's length
    public float spacing = 5f;

    // Vertical offset to position pickups slightly above the road
    public float yOffset = 1f;

    // Adds a context menu button in the Unity editor for testing
    [ContextMenu("Yollara Pickup Dağıt")]
    void PlacePickups()
    {
        // Find all GameObjects tagged as "Road"
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");

        foreach (GameObject road in roads)
        {
            // Get the Renderer to access the bounds of the road mesh
            Renderer rend = road.GetComponent<Renderer>();
            if (rend == null) continue;

            Vector3 min = rend.bounds.min;
            Vector3 max = rend.bounds.max;

            float length = rend.bounds.size.z;

            // Determine how many pickups to place based on spacing
            int count = Mathf.FloorToInt(length / spacing);

            for (int i = 0; i <= count; i++)
            {
                // Interpolate along the Z-axis of the road bounds
                float t = (float)i / count;
                float z = Mathf.Lerp(min.z, max.z, t);

                // Align pickups centered on the road's X position
                float x = rend.bounds.center.x;
                float y = road.transform.position.y + yOffset;

                Vector3 point = new Vector3(x, y, z);

                // Instantiate the pickup and make it a child of this object
                GameObject pickup = Instantiate(pickupPrefab, point, Quaternion.identity);
                pickup.transform.SetParent(this.transform);
            }
        }

        // Log message after placement is done
        UnityEngine.Debug.Log("Pickups placed centered on the roads.");
    }
}
