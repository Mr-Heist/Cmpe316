using UnityEngine;
using System.Collections;

public class ArabaKontrol : MonoBehaviour
{
    // References to each wheel collider component
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    // Driving and steering parameters
    public float motorForce = 800f;
    public float maxSteerAngle = 32f;

    // Rigidbody for physical interactions
    private Rigidbody rb;

    // Store the original motor force value
    private float originalMotorForce;

    // Audio source for engine sound simulation
    private AudioSource engineAudio;

    void Start()
    {
        // Get the Rigidbody component attached to the car
        rb = GetComponent<Rigidbody>();
        originalMotorForce = motorForce;

        // Get and configure the AudioSource for the engine sound
        engineAudio = GetComponent<AudioSource>();
        engineAudio.loop = true;
        engineAudio.playOnAwake = true;
        engineAudio.Play();

        // Lower the center of mass to make the car more stable
        rb.centerOfMass = new Vector3(0f, -1.2f, 0f);
        rb.angularDamping = 1f;

        // Adjust suspension and friction settings for each wheel
        AdjustSuspension(frontLeftCollider);
        AdjustSuspension(frontRightCollider);
        AdjustSuspension(rearLeftCollider);
        AdjustSuspension(rearRightCollider);

        AdjustFriction(frontLeftCollider);
        AdjustFriction(frontRightCollider);
        AdjustFriction(rearLeftCollider);
        AdjustFriction(rearRightCollider);
    }

    void FixedUpdate()
    {
        // Get player input for acceleration and steering
        float hiz = Input.GetAxis("Vertical") * motorForce;
        float donus = Input.GetAxis("Horizontal") * maxSteerAngle;

        // Calculate stability factor to reduce oversteering at high speeds
        float steerInput = Input.GetAxis("Horizontal");
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 50f);
        float stabilityFactor = 1f - Mathf.Abs(steerInput) * speedFactor;

        // Apply motor torque to rear wheels with stability adjustment
        rearLeftCollider.motorTorque = hiz * stabilityFactor;
        rearRightCollider.motorTorque = hiz * stabilityFactor;

        // Apply steering angle to front wheels
        frontLeftCollider.steerAngle = donus;
        frontRightCollider.steerAngle = donus;

        // Limit the car's maximum speed
        float maxSpeed = 30f;
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        // Add a small forward force for better acceleration control
        rb.AddForce(transform.forward * hiz * 0.005f, ForceMode.Acceleration);

        // Dynamically adjust engine sound pitch and volume
        float velocity = rb.linearVelocity.magnitude;
        float gasInput = Mathf.Abs(Input.GetAxis("Vertical"));

        float targetPitch = Mathf.Lerp(0.9f, 2.2f, velocity / 40f);
        float targetVolume = Mathf.Lerp(0.3f, 0.8f, gasInput);

        engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, targetPitch, Time.deltaTime * 5f);
        engineAudio.volume = Mathf.Lerp(engineAudio.volume, targetVolume, Time.deltaTime * 5f);
    }

    void Update()
    {
        // Pressing the R key resets the car's orientation and position
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCarRotation();
        }
    }

    // Resets the car's rotation to stand upright
    void ResetCarRotation()
    {
        // Keep current Y-rotation but reset pitch and roll
        Vector3 currentEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);

        // Perform a downward raycast to find ground and reposition the car
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 20f))
        {
            rb.position = hit.point + Vector3.up * 0.3f;
        }
        else
        {
            rb.position += Vector3.up * 1f;
        }

        // Stop all motion and wake the Rigidbody
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.WakeUp();

        AlignWheelColliders();
    }

    // Reset wheel collider rotations to default
    void AlignWheelColliders()
    {
        frontLeftCollider.transform.localRotation = Quaternion.identity;
        frontRightCollider.transform.localRotation = Quaternion.identity;
        rearLeftCollider.transform.localRotation = Quaternion.identity;
        rearRightCollider.transform.localRotation = Quaternion.identity;
    }

    // Set up suspension settings for better physics behavior
    void AdjustSuspension(WheelCollider wheel)
    {
        JointSpring spring = wheel.suspensionSpring;
        spring.spring = 50000;
        spring.damper = 8000;
        spring.targetPosition = 0.5f;
        wheel.suspensionSpring = spring;

        wheel.suspensionDistance = 0.3f;
    }

    // Configure the friction settings for each wheel
    void AdjustFriction(WheelCollider wheel)
    {
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        forwardFriction.extremumSlip = 0.4f;
        forwardFriction.extremumValue = 1f;
        forwardFriction.asymptoteSlip = 0.8f;
        forwardFriction.asymptoteValue = 0.5f;
        forwardFriction.stiffness = 1.8f;
        wheel.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.1f;
        sidewaysFriction.extremumValue = 1f;
        sidewaysFriction.asymptoteSlip = 0.3f;
        sidewaysFriction.asymptoteValue = 0.5f;
        sidewaysFriction.stiffness = 5f;
        wheel.sidewaysFriction = sidewaysFriction;
    }

    // Public method to apply a temporary speed boost
    public void ApplyImpulseBoost(float impulse, float duration)
    {
        StartCoroutine(TemporaryImpulse(impulse, duration));
    }

    // Coroutine that applies an impulse and then reverts damping settings
    private IEnumerator TemporaryImpulse(float impulse, float duration)
    {
        rb.AddForce(transform.forward * impulse, ForceMode.VelocityChange);

        float originalDrag = rb.linearDamping;
        float originalAngularDrag = rb.angularDamping;

        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;

        yield return new WaitForSeconds(duration);

        rb.linearDamping = originalDrag;
        rb.angularDamping = originalAngularDrag;
    }
}
