using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateInterface
{
    private PatrolAI ai;
    private NavMeshAgent agent;
    private Transform player;
    private float chaseRange;
    private float attackRange;

    public ChaseState(PatrolAI ai, NavMeshAgent agent, Transform player, float chaseRange, float attackRange)
    {
        // Initialize chase state parameters
        this.ai = ai;
        this.agent = agent;
        this.player = player;
        this.chaseRange = ai.chaseRange; // Assuming chaseRange is defined in PatrolAI
        this.attackRange = attackRange; // Assuming attackRange is defined in PatrolAI
    }

    public void EnterState()
    {
        Debug.Log("Entering Chase State");
        // Logic to start chasing the player, e.g., set speed, destination, etc.
        
    }

    public void UpdateState()
    {
        Debug.Log("Updating Chase State");
        // Logic for chasing the player, e.g., update destination, check distance, etc.
        float dist = Vector3.Distance(agent.transform.position, player.position);

        if (dist > chaseRange)
        {
            ai.TransitionToState(ai.patrolState);
            return;
        } else if (dist <= attackRange)
        {
            ai.TransitionToState(ai.attackState);
            return;
        }

        agent.SetDestination(player.position);
    }

    public void ExitState()
    {
        Debug.Log("Exiting Chase State");
        // Cleanup or reset parameters when exiting chase state
        agent.ResetPath(); // Stop the agent from moving        
    }
}
