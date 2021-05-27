using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    private Animator animator;
    private int      horizontalHash = 0;
    private int      verticalHash   = 0;
    private int      attackHash     = 0;

    private void Start() {
        animator = GetComponent<Animator>();

        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash   = Animator.StringToHash("Vertical");
        attackHash     = Animator.StringToHash("Attack");
    }

    private void Update() {
        float xAxis = Input.GetAxis("Horizontal") * 2.32f;
        float yAxis = Input.GetAxis("Vertical") * 5.66f;

        animator.SetFloat(horizontalHash, xAxis, 0.2f, Time.deltaTime);
        animator.SetFloat(verticalHash, yAxis, 1.0f, Time.deltaTime);
        if (Input.GetMouseButtonUp(0)) {
            animator.SetTrigger(attackHash);
        }
    }
}
