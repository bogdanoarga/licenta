using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NonVerbalResponse : MonoBehaviour
{
    public GameObject target;
    public float speed;

    private StudentBehavior studentBehavior;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool moveBack = false;
    private bool move = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        studentBehavior = GameObject.Find("StudentManager").GetComponent<StudentBehavior>();
        animator = GetComponent<Animator>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Vector3.Distance(transform.position, target.transform.position) <= 0.1 && !moveBack)
        {
            studentBehavior.NonVerbalTimerStart();

            moveBack = true;
            move = false;

            animator.SetBool("targetReached", true);
            animator.SetBool("animationPlay", true);
        }

        // initial position reached
        if (Vector3.Distance(transform.position, initialPosition) <= 0.1 && moveBack)
        {
            moveBack = false;
            this.transform.rotation = initialRotation;

            move = false;
            animator.SetBool("initialPositionReached", true);
            animator.SetBool("isMoving", false);
        }
    }

    public void StartMoving()
    {
        move = true;

        animator.SetBool("isMoving", true);

        animator.SetBool("initialPositionReached", false);
        animator.SetBool("targetReached", false);
        animator.SetBool("animationPlay", false);
    }
}
