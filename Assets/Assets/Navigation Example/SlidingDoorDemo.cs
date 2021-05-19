using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState { Closed, Opened, Animating }

public class SlidingDoorDemo : MonoBehaviour
{
    //Public Members
    public float            SlidingDistance = 4.0f;
    public float            Duration        = 1.5f;
    public AnimationCurve   Curve           = new AnimationCurve();

    //Private Members
    private Vector3         openPos         = Vector3.zero;
    private Vector3         closedPos       = Vector3.zero;
    private DoorState doorState = DoorState.Closed;

    void Start() {
        closedPos = transform.position;
        openPos   = closedPos + SlidingDistance * transform.right;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && doorState != DoorState.Animating) {
            StartCoroutine(AnimateDoor(doorState == DoorState.Closed ? DoorState.Opened : DoorState.Closed));
        }
    }

    private IEnumerator AnimateDoor(DoorState newState) {
        doorState = DoorState.Animating;
        float time = 0.0f;
        Vector3 startPos = newState == DoorState.Opened ? closedPos : openPos;
        Vector3 finalPos = newState == DoorState.Opened ? openPos : closedPos;

        while(time <= Duration) {
            float t = time / Duration;
            transform.position = Vector3.Lerp(startPos, finalPos, Curve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPos;
        doorState = newState;
    }
}
