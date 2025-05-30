using UnityEngine;
using UnityEngine.AI;

public class PoliceAI_Nav : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 200f;
    public float chaseSpeed = 15f;
    public float patrolSpeed = 10f;
    public float patrolRadius = 50f;
    public float loseSightDelay = 4f;

    private NavMeshAgent agent;
    private Vector3 patrolTarget;
    private float lastSeenTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // PlayerPrefs'ten zorluk seviyesini oku ve hızları ayarla
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
            default:
                chaseSpeed = 15f;
                patrolSpeed = 15f;
                break;
        }

        // NavMesh'e oturt
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        // Agent ayarları
        agent.baseOffset = 0f;
        agent.autoBraking = false;
        agent.acceleration = 999f;
        agent.angularSpeed = 999f;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.speed = patrolSpeed;

        ChooseNewPatrolPoint();
    }

    void Update()
    {
        if (!agent.enabled) return;

        agent.acceleration = 999f;
        agent.angularSpeed = 999f;

        if (CanSeePlayer())
        {
            lastSeenTime = Time.time;
            if (agent.speed != chaseSpeed)
                agent.speed = chaseSpeed;

            agent.SetDestination(player.position);
        }
        else if (Time.time - lastSeenTime < loseSightDelay)
        {
            if (agent.speed != chaseSpeed)
                agent.speed = chaseSpeed;

            agent.SetDestination(player.position);
        }
        else
        {
            Patrol();
        }
    }

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

        patrolTarget = player.position;
    }

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
                Debug.Log("Ray başka bir şeye çarptı: " + hit.transform.name);
        }
        else
        {
            Debug.DrawRay(rayOrigin, dirToPlayer * detectionRange, Color.red);
        }

        return false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            Vector3 awayFromPlayer = transform.position - collision.transform.position;
            awayFromPlayer.y = 0f;

            StartCoroutine(ApplyRecoil(awayFromPlayer.normalized));
        }
    }

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