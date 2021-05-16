using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentExample : MonoBehaviour
{
    //Inspector variables
    public AIWaypointNetwork WaypointNetwork = null;
    public int               CurrentIndex    = 0;
    public bool              HasPath         = false;
    public bool              PathPending     = false;
    public bool              PathStale       = false;
    public NavMeshPathStatus PathStatus      = NavMeshPathStatus.PathInvalid;

    //Private
    private NavMeshAgent navAgent = null;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        if (WaypointNetwork == null) return;

        SetNextDestination(false);
    }

    private void SetNextDestination(bool increment) {
        if (WaypointNetwork is null) return;

        int incStep = increment ? 1 : 0;

        int nextWaypointIndex = (CurrentIndex + incStep) >= WaypointNetwork.Waypoints.Count ? 0 : CurrentIndex + incStep;
        Transform nextWaypointTransform = WaypointNetwork.Waypoints[nextWaypointIndex];

        if (nextWaypointTransform != null) {
            CurrentIndex = nextWaypointIndex;
            navAgent.destination = nextWaypointTransform.position;
            return;
        }

        //not found a valid waypoint for this index
        CurrentIndex++;
    }

    void Update()
    {
        HasPath     = navAgent.hasPath;
        PathPending = navAgent.pathPending;
        PathStale   = navAgent.isPathStale;
        PathStatus  = navAgent.pathStatus;

        if ((!HasPath && !PathPending) || PathStatus == NavMeshPathStatus.PathInvalid) {
            SetNextDestination(true);
        }
        else if (PathStale) {
            SetNextDestination(false);
        }
    }
}
