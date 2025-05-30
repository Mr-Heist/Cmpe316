using UnityEngine;

public class CarRotator : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 20 * Time.deltaTime, 0); // rotate in y axis
    }
}