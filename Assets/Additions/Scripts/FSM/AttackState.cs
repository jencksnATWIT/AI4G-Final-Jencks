using UnityEngine;
using UnityEngine.AI;

public class AttackState : StateInterface
{
    private PatrolAI ai;
    private NavMeshAgent agent;
    private Transform player;
    private float attackRange;
    private float attackCooldown = 1.0f; // Time between attacks
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

    }
    
    public void ExitState()
    {
        Debug.Log("Exiting Attack State");
        // Cleanup or reset parameters when exiting attack state
    }
}
