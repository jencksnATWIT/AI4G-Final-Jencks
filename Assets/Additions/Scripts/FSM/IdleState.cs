using UnityEngine;
using UnityEngine.AI;

public class IdleState : StateInterface
{
    private PatrolAI ai;
    private NavMeshAgent agent;
    private Transform idlePosition;

    private float idleTimer = 0f;
    private float minIdleTime = 2.0f; // Minimum time to stay idle
    private float maxIdleTime = 5.0f; // Maximum time to stay idle
    private float realIdleTime;

    public IdleState(PatrolAI ai, NavMeshAgent agent, Transform idlePosition)
    {
        this.ai = ai;
        this.agent = agent;
        this.idlePosition = idlePosition;
    }

    public void EnterState()
    {
        Debug.Log("Entering Idle State");
        // Initialize idle parameters, e.g., set animations, etc.
        idleTimer = 0f;
        realIdleTime = Random.Range(minIdleTime, maxIdleTime);
        agent.SetDestination(idlePosition.position);
    }

    public void UpdateState()
    {
        Debug.Log("Updating Idle State");
        // Logic for idle state, e.g., waiting for input or events

        if (agent.remainingDistance < 0.1f && !agent.pathPending)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= realIdleTime)
            {
                ai.TransitionToState(ai.patrolState);
                return;
            }
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Idle State");
        // Cleanup or reset parameters when exiting idle state
    }
}
