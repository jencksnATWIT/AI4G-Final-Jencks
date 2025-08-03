using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class PatrolAI : MonoBehaviour
{
    [Header("AI Parameters")]
    public Transform[] waypoints; // Array of waypoints for patrolling
    public Transform player;
    public Transform idlePosition; // Position to go to when idle

    [Header("States")]
    public PatrolState patrolState { get; private set; }
    public IdleState idleState { get; private set; }
    public ChaseState chaseState { get; private set; }
    public AttackState attackState { get; private set; } // Assuming you have an AttackState class

    [Header("Settings")]
    public float chaseRange = 8f;
    public float playerCheckRate = 0.2f;
    public float attackRange = 1.5f; // Range at which the AI can attack the player
    public float attackDamage = 10f; // Damage dealt to the player

    private NavMeshAgent agent;
    private StateInterface currentState;
    private float lastPlayerCheckTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolState = new PatrolState(this, agent, waypoints);
        idleState = new IdleState(this, agent, idlePosition);
        chaseState = new ChaseState(this, agent, player, chaseRange, attackRange);
        attackState = new AttackState(this, agent, player, attackRange, attackDamage); // Assuming you have an AttackState class

        currentState = patrolState; // Start with the patrol state
        currentState.EnterState();

        //StartCoroutine(CheckForPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        //Player check?

        currentState?.UpdateState();
    }

    public void TransitionToState(StateInterface newState)
    {
        // Logic to transition to a new state
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = newState;
        currentState.EnterState();
    }

    public System.Collections.IEnumerator WaitAtWaypoint(float duration, System.Action onComplete)
    {
        //StartCoroutine(WaitCoroutine(duration, onComplete));
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
    }
    
}
