using System.Collections;
using UnityEngine;

public class StudentMeltdown : MonoBehaviour
{
    public float meltdownChance = 0.3f;
    public Animator animator;
    public Transform quietPlace;

    private StudentBehavior behavior;
    private StudentActions actions;

    private bool move = false;
    private bool moveBack = false;

    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public float speed = 1.5f;
    private Renderer[] renderers;

    private void Start()
    {
        behavior = FindObjectOfType<StudentBehavior>();
        actions = GetComponent<StudentActions>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        renderers = GetComponentsInChildren<Renderer>();
    }

    public bool ShouldHaveMeltdown()
    {
        return Random.value < meltdownChance;
    }

    public void StartCrying()
    {
        animator.SetTrigger("Cry");
        animator.SetBool("isMeltdown", true);
    }

    public void TriggerMeltdownPopup()
    {
        MeltdownPopupManager.Instance.ShowMessage("The student is not feeling well. Please help them.");
    }

    public void SendToQuietPlace()
    {
        animator.SetBool("meltdownConfirmed", true);
        StartCoroutine(MeltdownRoutine());
    }

    private IEnumerator MeltdownRoutine()
    {
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(() => animator.GetBool("meltdownConfirmed"));

        // Start walk to Quiet Place
        targetPosition = quietPlace.position;
        move = true;
        animator.SetBool("isMoving", true);
        Debug.Log("MeltdownRoutine: Walking to Quiet Place");

        yield return new WaitUntil(() => !move);

        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(0.5f);

        SetVisibility(false);
        yield return new WaitForSeconds(10f); // Stay hidden longer
        SetVisibility(true);

        // Face back toward chair
        Vector3 lookDir = (initialPosition - transform.position).normalized;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);

        // Walk back to seat
        targetPosition = initialPosition;
        moveBack = true;
        animator.SetBool("isMoving", true);
        Debug.Log("MeltdownRoutine: Walking back to seat");

        yield return new WaitUntil(() => !moveBack);

        animator.SetBool("isMoving", false);
        animator.SetBool("initialPositionReached", true);
        animator.SetTrigger("Sit");

        yield return new WaitForSeconds(2f);

        // Reset state
        animator.SetBool("initialPositionReached", false);
        animator.SetBool("isMeltdown", false);
        animator.SetBool("meltdownConfirmed", false);

        if (actions != null)
            actions.isInMeltdown = false;

        behavior.UnpauseTimer();
    }

    private void SetVisibility(bool isVisible)
    {
        foreach (var rend in renderers)
        {
            rend.enabled = isVisible;
        }
    }

    private void Update()
    {
        if (move)
        {
            Vector3 flatTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, flatTarget, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, flatTarget) <= 0.3f)
            {
                Debug.Log("Reached Quiet Place.");
                move = false;
            }
        }

        if (moveBack)
        {
            Vector3 flatTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, flatTarget, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, flatTarget) <= 0.3f)
            {
                Debug.Log("Returned to seat.");
                moveBack = false;
                transform.rotation = initialRotation;
            }
        }
    }
}
