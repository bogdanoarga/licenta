using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherMove : MonoBehaviour
{
    public Transform[] targets;  // Un array de transformări pentru țintele poziționale
    public float moveDuration = 4f;
    public float animationDuration = 4f;
    //float rotationDelay = 1f;

    private int currentIndex = 0;
    private Animator animator;
    public int frames = 3;


    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(AnimateAndMove());
    }

    

    IEnumerator AnimateAndMove()
    {
        while (currentIndex < targets.Length)
        {
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;
            Vector3 targetPosition = targets[currentIndex].position;
            Quaternion targetRotation = targets[currentIndex].rotation;

            float timeElapsed = 0f;

            // Activează animația de "Walking"
            animator.SetBool("Walking", true);

            while (timeElapsed < animationDuration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / animationDuration);
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / animationDuration);
                timeElapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            if (currentIndex == 0)
            {
                // Prima țintă, se activează animația de walking
                animator.SetBool("Idle", false);
                animator.SetBool("Walking", true);
               // yield return new WaitForSeconds(rotationDelay);
            }
            else if (currentIndex == 1)
            {
                // A doua țintă, se activează animația de raising hand și se declanșează tranzitia către walking
                animator.SetBool("Walking", false);
                animator.SetBool("RaisingHand", true);
                animator.SetTrigger("Walking");
            }
            else if (currentIndex == targets.Length - 1)
            {
                // Ultima țintă, se activează animația de raising hand și se oprește animația de walking
                animator.SetBool("Walking", false);
                animator.SetBool("RaisingHand", true);
            }
            else
            {
                // Restul țintelor, se dezactivează animația de raising hand și se activează animația de walking
                animator.SetBool("RaisingHand", false);
                animator.SetBool("Walking", true);
                animator.SetTrigger("Idle");
            }

            yield return new WaitForSeconds(1f);

            currentIndex++;
        }

        // S-a ajuns la ultimul target, oprește mișcarea și animațiile
        animator.SetTrigger("Idle");
        animator.SetBool("RaisingHand", false);
        animator.SetBool("Walking", false);
        animator.Play("Idle");
        animator.enabled = false;
        
    }
}

