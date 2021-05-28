using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentNoRootMotion : MonoBehaviour
{
    //Inspector variables
    public AIWaypointNetwork WaypointNetwork = null;
    public int               CurrentIndex    = 0;
    public bool              HasPath         = false;
    public bool              PathPending     = false;
    public bool              PathStale       = false;
    public bool              OnOffMeshLink   = false;
    public NavMeshPathStatus PathStatus      = NavMeshPathStatus.PathInvalid;
    public AnimationCurve    jumpCurve       = new AnimationCurve();

    //Private
    private NavMeshAgent navAgent         = null;
    private Animator     animator         = null;
    private float        originalMaxSpeed = 0f;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (navAgent) {
            originalMaxSpeed = navAgent.speed;
        }

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
        int turnOnSpot = 0;

        HasPath         = navAgent.hasPath;
        PathPending     = navAgent.pathPending;
        PathStale       = navAgent.isPathStale;
        PathStatus      = navAgent.pathStatus;

        Vector3 cross = Vector3.Cross(transform.forward, navAgent.desiredVelocity.normalized);
        float horizontal = cross.y < 0 ? -cross.magnitude : cross.magnitude;
        horizontal = Mathf.Clamp(horizontal * 2.32f, -2.32f, 2.32f);

        if (navAgent.velocity.magnitude < 1.0f && Vector3.Angle(transform.forward, navAgent.desiredVelocity) > 10.0f) {
            navAgent.speed = 0.1f;
            turnOnSpot = (int)Mathf.Sign(horizontal);
        }
        else {
            navAgent.speed = originalMaxSpeed;
            turnOnSpot = 0;
        }

        animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", navAgent.desiredVelocity.magnitude, 0.1f, Time.deltaTime);
        animator.SetInteger("TurnOnSpot", turnOnSpot);

        /*
        if (navAgent.isOnOffMeshLink) {
            StartCoroutine(Jump(1.0f, navAgent.destination, CurrentIndex));
            return;
        }
        */

        if ((navAgent.remainingDistance <= navAgent.stoppingDistance && !PathPending && !OnOffMeshLink) || PathStatus == NavMeshPathStatus.PathInvalid) {

            SetNextDestination(true);
        }
        else if (PathStale) {
            SetNextDestination(false);
        }
    }

    private IEnumerator Jump(float duration, Vector3 dest, int index) {
        OffMeshLinkData data = navAgent.currentOffMeshLinkData;
        Vector3 startPos     = navAgent.transform.position;
        Vector3 endpos       = data.endPos + (navAgent.baseOffset * Vector3.up);
        float time = 0;
        OnOffMeshLink = true;

        while (time <= duration) {
            float t = time/duration;
            navAgent.Warp(Vector3.Lerp(startPos, endpos, t) + jumpCurve.Evaluate(t) * Vector3.up);
            time += Time.deltaTime;
            yield return null;
        }
        
        navAgent.CompleteOffMeshLink();
        navAgent.destination = dest;
        CurrentIndex = index;
        OnOffMeshLink = false;
    }
}
