using UnityEngine;
using UnityEngine.AI;

public class AttackState : StateInterface
{
    private PatrolAI ai;
    private NavMeshAgent agent;
    private Transform player;
    private float attackRange;
    private float attackCooldown = 5.0f; // Time between attacks
    private float lastAttackTime;
    private float attackDamage; // Damage dealt to the player

    public AttackState(PatrolAI ai, NavMeshAgent agent, Transform player, float attackRange, float attackDamage)
    {
        // Initialize attack state parameters
        this.ai = ai;
        this.agent = agent;
        this.player = player;
        this.attackRange = attackRange; // Assuming attackRange is defined in PatrolAI
        this.attackDamage = attackDamage; // Assuming attackDamage is defined in PatrolAI
    }

    public void EnterState()
    {
        Debug.Log("Entering Attack State");
        // Logic to prepare for attacking, e.g., set speed, animations, etc.
    }

    public void UpdateState()
    {
        Debug.Log("Updating Attack State");
        // Logic for attacking the player, e.g., check distance, perform attack, etc.
        float dist = Vector3.Distance(agent.transform.position, player.position);

        if (dist > attackRange)
        {
            ai.TransitionToState(ai.chaseState);
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Perform attack
            Debug.Log("Attacking player!");
            // Here you would apply damage to the player
            // player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            lastAttackTime = Time.time; // Reset attack timer
        }

        agent.SetDestination(player.position); // Keep moving towards the player
    }

    public void ExitState()
    {
        Debug.Log("Exiting Attack State");
        // Cleanup or reset parameters when exiting attack state
        agent.ResetPath(); // Stop the agent from moving
    }
}
