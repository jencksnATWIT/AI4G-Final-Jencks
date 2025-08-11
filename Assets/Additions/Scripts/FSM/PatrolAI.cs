using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Linq;

public class PatrolAI : MonoBehaviour
{
    [Header("AI Parameters")]
    public Transform[] waypoints; // Array of waypoints for patrolling
    public Transform player;
    public Transform idlePosition; // Position to go to when idle
    public PerceptionModule perceptionModule; // Reference to the perception module

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

    [Header("Debugging")]
    public Material patrolMaterial;
    public Material idleMaterial;
    public Material chaseMaterial;
    public MeshRenderer meshRenderer;

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

        //Debug.Log("Running checkForWaypoints() in PatrolAI Start method.");
        //checkForWaypoints();
        //Invoke(nameof(checkForWaypoints), 1f); // Delay to ensure waypoints are set up
        //Debug.Log("Check for waypoints completed. Waypoints count: " + waypoints.Length);
        if (waypoints.Length > 0)
        {
            currentState = patrolState; // Start with the patrol state
            currentState.EnterState();
        }
        else
        {
            Debug.Log("Waiting for waypoint assignment");
        }
     }

    // Update is called once per frame
    void Update()
    {
        //Player check?
        if (perceptionModule.PlayerSeen())
        {
            //Debug.Log("Player seen by AI");
            if (currentState != chaseState || currentState != attackState)
            {
                TransitionToState(chaseState);
            }
        }

        currentState?.UpdateState();
    }

    public void TransitionToState(StateInterface newState)
    {
        // Logic to transition to a new state
        if (currentState != null)
        {
            currentState.ExitState();
        }

        if (newState == patrolState)
        {
            meshRenderer.material = patrolMaterial;
        }
        else if (newState == idleState)
        {
            meshRenderer.material = idleMaterial;
        }
        else if (newState == chaseState)
        {
            meshRenderer.material = chaseMaterial;
        }

        currentState = newState;
        currentState.EnterState();
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        if (waypoints.Length > 0)
        {
            currentState = patrolState;
            currentState.EnterState();
        }
    }

    void checkForWaypoints()
    {
        Debug.Log("Starting checkForWaypoints");
        if (waypoints == null)
        {
            Debug.LogWarning("Waypoints array is null. Initializing to empty array.");
            waypoints = new Transform[0];
        }

        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned to PatrolAI. Searching for EnvironmentMarkers in the scene.");

            // Find all environment markers in the scene
            EnvironmentMarker[] markers = FindObjectsByType<EnvironmentMarker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Debug.Log($"Found {markers.Length} EnvironmentMarkers in the scene.");

            // Additional debug for found markers
            foreach (var marker in markers)
            {
                Debug.Log($"Found marker: {marker.name} (active: {marker.gameObject.activeInHierarchy})");
            }

            // Assign the transforms of the markers to the waypoints array
            if (markers.Length == 0)
            {
                Debug.LogError("No EnvironmentMarkers found in the scene. Please add some or assign waypoints manually.");
                return;
            }

            waypoints = new Transform[markers.Length];
            Debug.Log($"Assigning {markers.Length} EnvironmentMarkers to waypoints.");

            // Assign the transforms of the markers to the waypoints array
            for (int i = 0; i < markers.Length; i++)
            {
                if (markers[i] == null)
                {
                    Debug.LogError($"Marker at index {i} is null!");
                    continue;
                }
                waypoints[i] = markers[i].transform;
                Debug.Log($"Waypoints assigned: {string.Join(", ", waypoints.Where(w => w != null).Select(w => w.name))}");
            }
            Debug.Log($"Waypoints assigned: {string.Join(", ", waypoints.Select(w => w.name))}");
        }
        else
        {
            Debug.Log($"Waypoints already assigned: {waypoints.Length} waypoints found.");
        }
    }

    /*
    void OnEnable()
    {
        DungeonGenerator.OnDungeonGenerated += checkForWaypoints;
    }

    void OnDisable()
    {
        DungeonGenerator.OnDungeonGenerated -= checkForWaypoints;
    } */

    public System.Collections.IEnumerator WaitAtWaypoint(float duration, System.Action onComplete)
    {
        //StartCoroutine(WaitCoroutine(duration, onComplete));
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
    }
    
}
