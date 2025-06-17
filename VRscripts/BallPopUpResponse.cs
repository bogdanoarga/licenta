using System.Collections;
using UnityEngine;

public class BallPopUpResponse : MonoBehaviour
{
    public GameObject popupCanvas;
    public GameObject student;
    public Transform ballTarget;
    public float speed = 1.5f;
    public float rotationSpeed = 5f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Animator animator;
    private AudioSource audioSource; //  NEW
    private bool movingToBall = false;
    private bool returningToChair = false;
    private Vector3 targetPosition;

    private void Start()
    {
        if (student == null || ballTarget == null)
        {
            Debug.LogError("BallPopUpResponse: Missing references (student or ballTarget)!");
            return;
        }

        animator = student.GetComponent<Animator>();
        audioSource = student.GetComponent<AudioSource>(); //  NEW

        if (audioSource == null)
        {
            Debug.LogError("BallPopUpResponse: AudioSource missing on student!");
        }

        originalPosition = student.transform.position;
        originalRotation = student.transform.rotation;

        Debug.Log("BallPopUpResponse: Initialized. Original position saved.");
    }

    public void OnNoClicked()
    {
        popupCanvas.SetActive(false);
        Debug.Log("BallPopUpResponse: No clicked. Closing popup.");
    }

    public void OnYesClicked()
    {
        popupCanvas.SetActive(false);
        StartCoroutine(SitRoutine());
    }

    private IEnumerator SitRoutine()
    {
        // Stand up and start walking
        animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(1.5f);

        // Move to ball
        targetPosition = ballTarget.position;
        movingToBall = true;
        Debug.Log("BallPopUpResponse: Walking toward the ball.");

        yield return new WaitUntil(() => !movingToBall);
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(0.1f);

        // Keep same sitting Y-level as original position
        student.transform.position = new Vector3(
            ballTarget.position.x,
            originalPosition.y,
            ballTarget.position.z
        );

        // Smooth 180° turn
        yield return StartCoroutine(SmoothRotate(student.transform, 1f));

        // Sit down
        animator.SetBool("initialPositionReached", true);
        Debug.Log("BallPopUpResponse: Sitting down on ball.");
        yield return new WaitForSeconds(2f);
        animator.SetBool("initialPositionReached", false);

        //  Play happy voice directly
        if (audioSource != null)
        {
            audioSource.Play();
            Debug.Log("BallPopUpResponse: Played happy sound!");
        }

        yield return new WaitForSeconds(8f); // Sit duration

        // Walk back
        animator.SetBool("isMoving", true);
        targetPosition = originalPosition;
        returningToChair = true;
        Debug.Log("BallPopUpResponse: Returning to chair.");

        yield return new WaitUntil(() => !returningToChair);
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(0.1f);

        // Sit back at chair
        animator.SetBool("initialPositionReached", true);
        Debug.Log("BallPopUpResponse: Sitting down at original chair.");
        yield return new WaitForSeconds(1.2f);
        animator.SetBool("initialPositionReached", false);

        student.transform.rotation = originalRotation;
    }

    private IEnumerator SmoothRotate(Transform obj, float duration)
    {
        Quaternion startRotation = obj.rotation;
        Quaternion endRotation = Quaternion.Euler(0f, startRotation.eulerAngles.y + 180f, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            obj.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.rotation = endRotation;
    }

    private void Update()
    {
        if (movingToBall)
            MoveAndRotate(student, targetPosition, ref movingToBall);

        if (returningToChair)
            MoveAndRotate(student, targetPosition, ref returningToChair);
    }

    private void MoveAndRotate(GameObject obj, Vector3 destination, ref bool flag)
    {
        Vector3 flatTarget = new Vector3(destination.x, obj.transform.position.y, destination.z);
        Vector3 direction = (flatTarget - obj.transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        obj.transform.position = Vector3.MoveTowards(obj.transform.position, flatTarget, speed * Time.deltaTime);

        if (Vector3.Distance(obj.transform.position, flatTarget) <= 0.2f)
        {
            flag = false;
        }
    }
}
