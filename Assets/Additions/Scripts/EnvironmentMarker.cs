using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum EnvironmentTagType { SafeZone, Chokepoint }

public class EnvironmentMarker : MonoBehaviour
{
    public EnvironmentTagType tagType; // Enum to define the type of environment marker

    void Awake()
    {
        Debug.Log($"{name} is a valid EnvironmentMarker!", this);
    }

    private void OnDrawGizmos()
    {
        switch (tagType)
        {
            case EnvironmentTagType.SafeZone:
                Gizmos.color = Color.green;
                break;
            case EnvironmentTagType.Chokepoint:
                Gizmos.color = Color.red;
                break;
        }
        Gizmos.DrawCube(transform.position + Vector3.up * 0.5f, Vector3.one);
    }
}
