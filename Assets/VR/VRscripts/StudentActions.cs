using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentActions : MonoBehaviour
{
    // Array of possible responses. The first is verbal, the rest are animation triggers.
    public static string[] responses = { "buna dimineata", "animationPlay", "animationPlay", "animationPlay" };

    // Flags to indicate whether each corresponding response is animated.
    public bool[] animatedResponse = { false, true, true, true };

    // Index of the current response in the sequence.
    private int currentResponse = 0;

    // Reference to the AudioManager component (for playing sounds).
    private AudioManager audioManager;

    // Flags to handle eye contact logic.
    private bool eyeContactTime = false;
    private bool eyeContact = false;

    // Whether the student can speak.
    public bool isVerbal;

    // Whether the student is currently in meltdown mode.
    public bool isInMeltdown = false;

    // Reference to the Animator component (for playing animations).
    private Animator animator;

    // Called when the GameObject is first initialized
    public void Start()
    {
        audioManager = GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
    }

    // Called when it's time for the student to respond
    public bool PlayVoiceLine()
    {
        // Skip response if in meltdown
        if (isInMeltdown)
        {
            Debug.Log("Skipping voice line because student is in meltdown: " + gameObject.name);
            return false;
        }

        // Check if a meltdown should be triggered now
        StudentMeltdown meltdown = GetComponent<StudentMeltdown>();
        bool meltdownTriggered = meltdown != null && meltdown.ShouldHaveMeltdown();

        if (meltdownTriggered)
        {
            isInMeltdown = true;
            Debug.Log("MELTDOWN TRIGGERED for " + gameObject.name);
            FindObjectOfType<StudentBehavior>().PauseTimer(); // Pause any student timers

            animator.SetBool("isMeltdown", true); // Trigger meltdown animation
            meltdown.StartCrying(); // Start crying logic
            meltdown.TriggerMeltdownPopup(); // Show meltdown popup
            return false;
        }

        // --- NORMAL BEHAVIOR ---

        // If verbal, or not using an animated response, play the voice line
        if (isVerbal || !GetAnimatedResponse())
        {
            audioManager.Play(responses[currentResponse]);
        }

        // If it's an animated response, trigger animation
        if (GetAnimatedResponse())
        {
            animator.SetBool("animationPlay", true);
        }

        // If nonverbal and not melting down, start the nonverbal behavior after short delay
        if (!isVerbal && !meltdownTriggered)
        {
            StartCoroutine(DelayedNonVerbalStart());
        }
        else if (!isVerbal && meltdownTriggered)
        {
            Debug.Log("Nonverbal skipped because meltdown was triggered.");
        }

        // Move to the next response in the list if not at the end
        if (currentResponse < responses.Length - 1)
            currentResponse++;

        // Reset eye contact trigger
        eyeContactTime = false;

        // Return whether eye contact was made (can be used externally)
        return eyeContact;
    }

    // Delay nonverbal action slightly to let meltdown logic finish
    private IEnumerator DelayedNonVerbalStart()
    {
        yield return new WaitForSeconds(4f); // Short wait

        // If not in meltdown anymore, start nonverbal response
        if (!isInMeltdown)
        {
            var nonVerbal = GetComponent<NonVerbalResponse>();
            if (nonVerbal != null)
            {
                nonVerbal.StartMoving(); // Start the pointing or walking behavior
            }
            else
            {
                Debug.Log("No NonVerbalResponse script is attached to the game object");
            }
        }
        else
        {
            Debug.Log("Skipping NonVerbal because meltdown started during delay.");
        }
    }

    // Trigger when the student is expected to make eye contact
    public void StartEyeContact()
    {
        eyeContactTime = true;
    }

    // Confirm that eye contact occurred
    public void SetEyeContact()
    {
        if (eyeContactTime)
        {
            eyeContact = true;
        }
    }

    // Clear the eye contact tracking
    public void ResetEyeContact()
    {
        eyeContactTime = false;
        eyeContact = false;
    }

    // Restart the voice line sequence from the beginning
    public void ResetVoiceLines()
    {
        currentResponse = 0;
    }

    // Force the student to do the nonverbal movement (if not in meltdown)
    public void StartMoving()
    {
        var nonVerbal = GetComponent<NonVerbalResponse>();
        if (!isInMeltdown && nonVerbal != null)
        {
            nonVerbal.StartMoving();
        }
    }

    // Check if current response should trigger an animation
    public bool GetAnimatedResponse()
    {
        return animatedResponse[currentResponse];
    }

    // Called when meltdown ends – reset flags and animations
    public void EndMeltdown()
    {
        isInMeltdown = false;
        animator.SetBool("isMeltdown", false);
    }
}
