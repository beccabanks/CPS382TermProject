using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AIWaypointDriver : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float waypointRadius = 5f;

    [Header("Driving")]
    public float maxSpeed = 20f;
    public float acceleration = 10f;
    public float turnSpeed = 5f;
    public float brakeForce = 15f;

    [Header("Lane Variation")]
    public float laneWidth = 3f;
    public float laneDepth = 1.5f;
    public float offsetChangeChance = 0.2f;

    [Header("Terrain")]
    public LayerMask terrainLayer;
    public float raycastHeight = 10f;
    public float raycastDistance = 50f;

    [Header("Track Bounds")]
    public float maxDistanceFromCenter = 8f;

    [Header("Driver Personality")]
    [Range(0f, 1f)] public float aggression = 0.5f;
    [Range(0f, 1f)] public float skill = 0.7f;
    [Range(0f, 1f)] public float randomness = 0.5f;

    [Header("Noise Drift")]
    public bool usePerlinNoise = true;
    public float noiseScale = 0.5f;

    private Rigidbody rb;
    private int currentWaypoint = 0;
    private Vector3 currentOffset;
    private float baseSpeed;
    private float aiSeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        baseSpeed = maxSpeed;
        aiSeed = Random.Range(0f, 1000f);

        GenerateNewOffset();
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;

        Transform wp = waypoints[currentWaypoint];

        // Check if waypoint reached
        if (Vector3.Distance(transform.position, wp.position) < waypointRadius)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;

            if (Random.value < offsetChangeChance)
                GenerateNewOffset();
        }

        Vector3 target = GetTargetPosition(wp);
        MoveTowards(target);
    }

    void GenerateNewOffset()
    {
        float width = laneWidth * randomness;
        float depth = laneDepth * randomness;

        currentOffset = new Vector3(
            Random.Range(-width, width),
            0,
            Random.Range(-depth, depth)
        );
    }

    Vector3 GetTargetPosition(Transform wp)
    {
        Vector3 target = wp.position + currentOffset;

        // Optional Perlin noise drift
        if (usePerlinNoise)
        {
            float noise = Mathf.PerlinNoise(Time.time * noiseScale, aiSeed) - 0.5f;
            target += transform.right * noise * laneWidth * randomness;
        }

        // Clamp to track center if too far
        float dist = Vector3.Distance(transform.position, wp.position);
        if (dist > maxDistanceFromCenter)
        {
            target = Vector3.Lerp(target, wp.position, 0.7f);
        }

        // Project onto terrain
        Ray ray = new Ray(target + Vector3.up * raycastHeight, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, terrainLayer))
        {
            target = hit.point;
        }

        return target;
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        // Steering
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        float steer = Mathf.Clamp(angle / 45f, -1f, 1f);

        // Apply rotation
        transform.Rotate(Vector3.up, steer * turnSpeed * 100f * Time.fixedDeltaTime);

        // Speed control based on turn angle
        float speedFactor = Mathf.Lerp(1f, 0.5f, Mathf.Abs(steer));
        float targetSpeed = baseSpeed * speedFactor * Mathf.Lerp(0.8f, 1.2f, aggression);

        float currentSpeed = rb.linearVelocity.magnitude;

        // Accelerate or brake
        if (currentSpeed < targetSpeed)
        {
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(-transform.forward * brakeForce, ForceMode.Acceleration);
        }

        // Extra stability (skill reduces sliding)
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, transform.forward * currentSpeed, skill * 0.1f);
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
                Gizmos.DrawWireSphere(waypoints[i].position, waypointRadius);
        }
    }
}

