using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NonVerbalResponse : MonoBehaviour
{
    public GameObject target;
    public float speed;
    public float delayBeforeMovement = 4f; // delay before starting movement
    public float pointDuration = 2f; // how long to hold point

    private StudentBehavior studentBehavior;
    private StudentActions studentActions;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool moveBack = false;
    private bool move = false;
    private bool hasPointed = false; // guard to run pointing sequence once
    private bool inSequence = false; // ensure only one nonverbal run at a time

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        studentBehavior = GameObject.Find("StudentManager").GetComponent<StudentBehavior>();
        animator = GetComponent<Animator>();
        studentActions = GetComponent<StudentActions>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Skip movement during meltdown
        if (studentActions != null && studentActions.isInMeltdown)
            return;

        if (move)
        {

            if (!moveBack)
            {
                this.transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            }
            else
            {
                this.transform.position = Vector3.MoveTowards(transform.position, initialPosition, speed * Time.deltaTime);
                this.transform.rotation = new Quaternion(this.transform.rotation.x, 180, this.transform.rotation.z, this.transform.rotation.w);
            }
        }

        // target reached
        if (Vector3.Distance(transform.position, target.transform.position) <= 0.1 && !moveBack && !hasPointed)
        {
            // arrived at board: stop and point
            move = false;
            hasPointed = true;
            animator.SetBool("targetReached", true);
            animator.SetBool("animationPlay", true);
            StartCoroutine(PointAndReturn());
        }

        // initial position reached
        if (Vector3.Distance(transform.position, initialPosition) <= 0.1 && moveBack)
        {
            moveBack = false;
            this.transform.rotation = initialRotation;

            move = false;
            animator.SetBool("initialPositionReached", true);
            animator.SetBool("isMoving", false);
            inSequence = false; // allow future runs
            // start next student's timer now that nonverbal sequence is fully done
            studentBehavior.StartTimer();
        }
    }

    // Start moving with delay
    public void StartMoving()
    {
        if (studentActions != null && studentActions.isInMeltdown)
            return;
        if (inSequence) return; // already running
        inSequence = true;
        hasPointed = false; // reset sequence
        moveBack = false; // reset return flag
        StartCoroutine(DelayedMovement());
    }

    private IEnumerator DelayedMovement()
    {
        yield return new WaitForSeconds(delayBeforeMovement);
        if (studentActions != null && studentActions.isInMeltdown)
            yield break;
        move = true;

        animator.SetBool("isMoving", true);
        animator.SetBool("initialPositionReached", false);
        animator.SetBool("targetReached", false);
        animator.SetBool("animationPlay", false);
    }

    private IEnumerator PointAndReturn()
    {
        // hold pointing
        yield return new WaitForSeconds(pointDuration);
        // lower finger and prepare to return
        animator.SetBool("animationPlay", false);
        animator.SetBool("targetReached", false);
        moveBack = true;
        move = true;
        animator.SetBool("isMoving", true);
    }

    // Reset sequence state for retry
    public void ResetSequence()
    {
        StopAllCoroutines();
        move = false;
        moveBack = false;
        hasPointed = false;
        inSequence = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        animator.SetBool("isMoving", false);
        animator.SetBool("initialPositionReached", false);
        animator.SetBool("targetReached", false);
        animator.SetBool("animationPlay", false);
    }

    // Expose nonverbal in-sequence state
    public bool IsInSequence
    {
        get { return inSequence; }
    }
}
