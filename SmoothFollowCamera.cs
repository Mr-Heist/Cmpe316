using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;

    private Vector3 originalOffset;

    void Start()
    {
        originalOffset = offset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // C tuşuna basılıysa offset'i ters çevir
        if (Input.GetKey(KeyCode.C))
        {
            offset.z = Mathf.Abs(originalOffset.z); // öne bak
        }
        else
        {
            offset.z = originalOffset.z; // normale dön
        }

        Vector3 desiredPosition = target.position
                                + target.forward * offset.z
                                + target.up * offset.y
                                + target.right * offset.x;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
