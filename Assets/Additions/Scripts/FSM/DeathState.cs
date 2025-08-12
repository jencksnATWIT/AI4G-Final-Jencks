using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class DeathState : StateInterface
{
    [Header("Agent Parameters")]
    private PatrolAI ai;
    private NavMeshAgent agent;

    public DeathState(PatrolAI ai, NavMeshAgent agent)
    {
        this.ai = ai;
        this.agent = agent;
    }

    public void EnterState()
    {
        Debug.Log("Entering Death State");
        // Logic for entering the death state, e.g., stopping movement, playing death animation
        agent.isStopped = true;
        // Optionally play a death animation or sound here
    }

    public void UpdateState()
    {
        Object.Destroy(ai.gameObject);
        Debug.Log("Updating Death State");
        // Logic for updating the death state, e.g., waiting for a certain time before respawning or destroying the object
    }

    public void ExitState()
    {
        Debug.Log("Exiting Death State");
        // Logic for exiting the death state, e.g., resetting parameters or transitioning to a respawn state
        // This could also involve destroying the AI object or resetting its position
        Object.Destroy(ai.gameObject); // Example: Destroy the AI object
    }
}
