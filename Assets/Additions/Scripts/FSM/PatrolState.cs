using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class PatrolState : StateInterface
{
    [Header("Agent Parameters")]
    private PatrolAI ai;
    private NavMeshAgent agent;


    [Header("Patrol State Parameters")]
    [Tooltip("Waypoints for the patrol route")]
    private Transform[] waypoints;
    [Tooltip("Duration to pause at each waypoint")]
    [SerializeField] private float pauseDuration = 2.0f;
    [Tooltip("Speed of the patrol movement")]
    public float patrolSpeed = 2.0f;

    private int index = 0;

    public PatrolState(PatrolAI ai, NavMeshAgent agent, Transform[] waypoints)
    {
        this.ai = ai;
        this.agent = agent;
        this.waypoints = waypoints;
    }

    public void EnterState()
    {
        Debug.Log("Entering Patrol State");
        // Initialize patrol parameters, e.g., set waypoints, speed, etc.
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints set for patrol.");
            return;
        }

        agent.SetDestination(waypoints[index].position);
    }

    public void UpdateState()
    {
        Debug.Log("Updating Patrol State");
        // Logic for patrolling, e.g., moving between waypoints

        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            // Wait at the current waypoint
            ai.StartCoroutine(ai.WaitAtWaypoint(pauseDuration, () =>
            {
                index = (index + 1) % waypoints.Length; // Move to the next waypoint
                agent.SetDestination(waypoints[index].position);
            }));
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Patrol State");
        // Clean up or reset patrol parameters if necessary
    }
}
