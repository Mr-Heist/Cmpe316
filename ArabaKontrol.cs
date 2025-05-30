using UnityEngine;
using System.Collections;

public class ArabaKontrol : MonoBehaviour
{
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    public float motorForce = 1500f;
    public float maxSteerAngle = 30f;

    private Rigidbody rb;
    private float originalMotorForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalMotorForce = motorForce;
    }

    void FixedUpdate()
    {
        float hiz = Input.GetAxis("Vertical") * motorForce;
        float donus = Input.GetAxis("Horizontal") * maxSteerAngle;

        rearLeftCollider.motorTorque = hiz;
        rearRightCollider.motorTorque = hiz;

        frontLeftCollider.steerAngle = donus;
        frontRightCollider.steerAngle = donus;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCarRotation();
        }
    }

    void ResetCarRotation()
    {
        Vector3 currentEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 20f))
        {
            rb.position = hit.point + Vector3.up * 0.3f;
        }
        else
        {
            rb.position += Vector3.up * 1f;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.WakeUp();

        AlignWheelColliders();
    }

    void AlignWheelColliders()
    {
        frontLeftCollider.transform.localRotation = Quaternion.identity;
        frontRightCollider.transform.localRotation = Quaternion.identity;
        rearLeftCollider.transform.localRotation = Quaternion.identity;
        rearRightCollider.transform.localRotation = Quaternion.identity;
    }

    // ✅ BOOST FONKSİYONU (süreli impulse boost)
    public void ApplyImpulseBoost(float impulse, float duration)
    {
        StartCoroutine(TemporaryImpulse(impulse, duration));
    }

    private IEnumerator TemporaryImpulse(float impulse, float duration)
    {
        // Anlık hız ver
        rb.AddForce(transform.forward * impulse, ForceMode.VelocityChange);

        // Sürtünmeleri azalt
        float originalDrag = rb.linearDamping;
        float originalAngularDrag = rb.angularDamping;

        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;

        yield return new WaitForSeconds(duration);

        // Eski değerlere dön
        rb.linearDamping = originalDrag;
        rb.angularDamping = originalAngularDrag;
    }
}
