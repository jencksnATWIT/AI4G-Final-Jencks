using UnityEngine;
using UnityEngine.AI;

public class SearchState : StateInterface
{
    private PatrolAI ai;
    private NavMeshAgent agent;
    public Transform point;
    private float searchTime;
    private float searchTimer = 0f;
    private Vector3 rotation = new Vector3(0, 0, 0); // Direction to look from the search point

    public SearchState(PatrolAI ai, NavMeshAgent agent, Transform point, float searchTime)
    {
        // Initialize search state parameters
        this.ai = ai;
        this.agent = agent;
        this.point = point;
        this.searchTime = searchTime; // Assuming searchRange is defined in PatrolAI
    }

    public void EnterState()
    {
        Debug.Log("Entering Search State");
        // Logic to start searching for the player, e.g., set speed, destination, etc.
        agent.SetDestination(point.position);
        searchTimer = 0f; // Reset search timer
        rotation.y = Random.Range(-30, 30); // Randomize rotation direction
    }

    public void UpdateState()
    {
        Debug.Log("Updating Search State");
        // Logic for searching the player, e.g., update destination, check distance, etc.

        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            searchTimer += Time.deltaTime;
            if (searchTimer >= searchTime)
            {
                // After searching for the specified time, transition to another state
                ai.TransitionToState(ai.patrolState);
                return;
            }

            // Rotate around the search point
            agent.transform.Rotate(rotation * Time.deltaTime);
            
        }

    }

    public void ExitState()
    {
        Debug.Log("Exiting Search State");
        // Cleanup or reset parameters when exiting search state
        agent.ResetPath(); // Stop the agent from moving
        searchTimer = 0f; // Reset search timer
    }

    
}