using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : Node
{
    public WaypointNode[] nextWaypoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<SearchAgent> (out SearchAgent searchAgent))
        {
            if (searchAgent.targetNode == this)
            {
                searchAgent.targetNode = nextWaypoint[Random.Range(0, nextWaypoint.Length)];
            }
        }
    }
}
