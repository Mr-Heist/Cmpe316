
using UnityEngine;
using UnityEngine.AI;

public class PoliceAI_Nav : MonoBehaviour
{
    // Reference to the player object
    public Transform player;

    // Vision and behavior parameters
    public float detectionRange = 200f;
    public float chaseSpeed = 15f;
    public float patrolSpeed = 10f;
    public float patrolRadius = 50f;
    public float loseSightDelay = 4f;

    // Internal state variables
    private NavMeshAgent agent;
    private Vector3 patrolTarget;
    private float lastSeenTime;

    // Audio source for siren sound
    private AudioSource sirenAudio;

    void Start()
    {
        // Get the NavMeshAgent and AudioSource components
        agent = GetComponent<NavMeshAgent>();
        sirenAudio = GetComponent<AudioSource>();

        // Read difficulty setting and adjust speeds accordingly
        string difficulty = PlayerPrefs.GetString("difficulty", "easy");

        switch (difficulty)
        {
            case "easy":
                chaseSpeed = 15f;
                patrolSpeed = 15f;
                break;
            case "medium":
                chaseSpeed = 20f;
                patrolSpeed = 15f;
                break;
            case "hard":
                chaseSpeed = 25f;
                patrolSpeed = 20f;
                break;
        }

        // Ensure the police object is on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            transform.position = hit.position;

        // Configure NavMeshAgent settings for smooth and responsive movement
        agent.baseOffset = 0f;
        agent.autoBraking = false;
        agent.acceleration = 999f;
        agent.angularSpeed = 999f;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.speed = patrolSpeed;

        // Choose an initial patrol destination
        ChooseNewPatrolPoint();
    }

    void Update()
    {
        // If the NavMeshAgent is disabled, skip logic
        if (!agent.enabled) return;

        // Continuously reset acceleration and turning speed for responsiveness
        agent.acceleration = 999f;
        agent.angularSpeed = 999f;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Play siren only when close to the player
        if (distanceToPlayer <= 30f)
        {
            if (!sirenAudio.isPlaying)
                sirenAudio.Play();
        }
        else
        {
            if (sirenAudio.isPlaying)
                sirenAudio.Stop();
        }

        // If player is visible, chase them
        if (CanSeePlayer())
        {
            lastSeenTime = Time.time;

            if (agent.speed != chaseSpeed)
                agent.speed = chaseSpeed;

            agent.SetDestination(player.position);
        }
        // If player was seen recently, continue chasing
        else if (Time.time - lastSeenTime < loseSightDelay)
        {
            if (agent.speed != chaseSpeed)
                agent.speed = chaseSpeed;

            agent.SetDestination(player.position);
        }
        // Otherwise, return to patrolling
        else
        {
            Patrol();
        }
    }

    // Handles patrolling behavior when not chasing
    void Patrol()
    {
        if (!agent.enabled) return;

        if (agent.speed != patrolSpeed)
            agent.speed = patrolSpeed;

        if (Vector3.Distance(transform.position, patrolTarget) < 3f)
        {
            ChooseNewPatrolPoint();
        }

        agent.SetDestination(patrolTarget);
    }

    // Randomly selects a point within patrol radius that is on the NavMesh
    void ChooseNewPatrolPoint()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = new Vector3(randomCircle.x, transform.position.y, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 5f, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                return;
            }
        }

        // Fallback: go toward the player's last known position
        patrolTarget = player.position;
    }

    // Checks if there is line of sight to the player using a SphereCast
    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > detectionRange)
            return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        RaycastHit hit;
        int mask = LayerMask.GetMask("Default", "Player");

        if (Physics.SphereCast(rayOrigin, 1f, dirToPlayer, out hit, detectionRange, mask))
        {
            Debug.DrawRay(rayOrigin, dirToPlayer * detectionRange, Color.green);

            if (hit.transform.CompareTag("Player"))
                return true;
            else
                Debug.Log("Ray hit something else: " + hit.transform.name);
        }
        else
        {
            Debug.DrawRay(rayOrigin, dirToPlayer * detectionRange, Color.red);
        }

        return false;
    }

    // Handles collision with the player
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            // Calculate pushback direction and apply recoil
            Vector3 awayFromPlayer = transform.position - collision.transform.position;
            awayFromPlayer.y = 0f;

            StartCoroutine(ApplyRecoil(awayFromPlayer.normalized));
        }
    }

    // Applies recoil force and temporarily disables NavMeshAgent
    System.Collections.IEnumerator ApplyRecoil(Vector3 direction)
    {
        float originalSpeed = agent.speed;
        agent.speed = 0f;

        agent.enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        rb.AddForce(direction * 300f);
        yield return new WaitForSeconds(0.25f);

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        agent.enabled = true;

        // Smoothly restore movement speed over time
        float t = 0f;
        float restoreDuration = 1f;
        while (t < restoreDuration)
        {
            agent.speed = Mathf.Lerp(0f, originalSpeed, t / restoreDuration);
            t += Time.deltaTime;
            yield return null;
        }

        agent.speed = originalSpeed;
    }
}
