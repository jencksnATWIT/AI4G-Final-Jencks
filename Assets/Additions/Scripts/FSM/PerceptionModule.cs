using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class PerceptionModule : MonoBehaviour
{
    [Header("Perception Settings")]
    [Tooltip("Range within which the AI can see the player")]
    public float sightRange = 10f;
    [Tooltip("Angle of vision for the AI")]
    public float FOV = 90f;
    [Tooltip("Origin point for sight")]
    public Transform eyePosition;
    [Tooltip("Range within which the AI can hear sounds")]
    public float hearingRange = 5f;
    [Tooltip("Layer mask for detecting the player")]
    public LayerMask playerLayer;
    [Tooltip("Layer mask for obstacles that block sight")]
    public LayerMask obstacleLayer;

    [Header("Agent Parameters")]
    public PatrolAI ai; // Reference to the AI controller
    public NavMeshAgent agent; // Reference to the NavMeshAgent component

    public bool PlayerSeen()
    {
        Collider[] hits = Physics.OverlapSphere(eyePosition.position, sightRange, playerLayer);
        foreach (Collider hit in hits)
        {
            //Transform player = hit.transform;
            Vector3 dir = (hit.transform.position - eyePosition.position).normalized;
            float angle = Vector3.Angle(eyePosition.forward, dir);

            if (angle < FOV / 2f)
            {
                // Check if there are any obstacles blocking the line of sight
                if (!Physics.Linecast(eyePosition.position, hit.transform.position, obstacleLayer))
                {
                    Debug.Log("Player seen by AI");
                    return true; // Player is within sight range and not blocked by obstacles
                }
            }
        }
        return false;
    }

    public void SoundHeard(Vector3 origin)
    {
        // Logic to handle sound detection, e.g., transition to search state
        Debug.Log("Sound heard at: " + origin);
        //ai.searchState.point.position = origin;
        //ai.TransitionToState(ai.searchState);
    }

    void OnDrawGizmosSelected()
    {
        if (eyePosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyePosition.position, sightRange);

            Vector3 left = Quaternion.Euler(0, -FOV / 2, 0) * eyePosition.forward * sightRange;
            Vector3 right = Quaternion.Euler(0, FOV / 2, 0) * eyePosition.forward * sightRange;

            Gizmos.DrawLine(eyePosition.position, eyePosition.position + left);
            Gizmos.DrawLine(eyePosition.position, eyePosition.position + right);
        }
    }
}
