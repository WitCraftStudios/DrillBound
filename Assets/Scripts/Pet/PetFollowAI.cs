using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class PetFollowAI : MonoBehaviour
{
    [Tooltip("Reference to the player transform.")]
    public Transform player;

    [Tooltip("Desired distance to keep from the player.")]
    public float followDistance = 3f;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = followDistance + 0.5f;
        agent.updateRotation = true;
        agent.autoBraking = true;
        agent.radius = 0.4f;
        agent.avoidancePriority = 50;
        // Recommended for smoothness (can also set in Inspector):
        // agent.acceleration = 16;
        // agent.angularSpeed = 360;
        // agent.speed = 3.5f;
    }

    void Start()
    {
        if (player == null)
            Debug.LogError("Player transform not assigned in PetFollowAI");
    }

    void Update()
    {
        if (player == null) return;

        // Ensure pet stays on NavMesh
        NavMeshHit hit;
        float maxDistance = 2.0f; // How far to search for a valid NavMesh position
        if (!NavMesh.SamplePosition(transform.position, out hit, maxDistance, NavMesh.AllAreas))
        {
            // Not on NavMesh, try to warp to nearest valid position
            if (NavMesh.SamplePosition(transform.position, out hit, 10.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            // else: could not find a valid NavMesh position nearby
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Always set destination if farther than stopping distance
        if (distance > agent.stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    // Optional: Set the player at runtime
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}
