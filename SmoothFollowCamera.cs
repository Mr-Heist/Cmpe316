using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    // Target to follow (e.g., the player)
    public Transform target;

    // Offset from the target's position (local space)
    public Vector3 offset = new Vector3(0, 5, -10);

    // How quickly the camera follows the target
    public float smoothSpeed = 5f;

    // Stores the original offset for toggling view
    private Vector3 originalOffset;

    void Start()
    {
        // Save the original offset at the start
        originalOffset = offset;
    }

    void LateUpdate()
    {
        // Do nothing if no target is assigned
        if (target == null) return;

        // If the C key is held down, reverse the Z offset to look from the front
        if (Input.GetKey(KeyCode.C))
        {
            offset.z = Mathf.Abs(originalOffset.z);
        }
        else
        {
            // Revert to original third-person view
            offset.z = originalOffset.z;
        }

        // Calculate the desired position based on the target's local directions
        Vector3 desiredPosition = target.position
                                + target.forward * offset.z
                                + target.up * offset.y
                                + target.right * offset.x;

        // Smoothly interpolate from current position to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Make the camera look at the target
        transform.LookAt(target);
    }
}
