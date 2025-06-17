using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro

[System.Serializable]
public class TourPoint
{
    public Transform target;
    public float stayTime = 3f;
    public float moveSpeed = 1f;
    public float rotateSpeed = 100f;
    public string descriptionText; // Text for this point
    public AudioClip voiceOverClip;  //  Voice narration clip
}

public class CameraTour : MonoBehaviour
{
    public TourPoint[] tourPoints;
    public Transform rigTransform;
    public CanvasGroup fadeCanvasGroup; // Optional for fade
    public TextMeshProUGUI descriptionTextUI; // UI text
    public AudioSource audioSource; //  Drag your TourRigMover AudioSource here

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start()
    {
        if (rigTransform == null)
        {
            Debug.LogError("Rig Transform is not assigned!");
            return;
        }

        originalPosition = rigTransform.position;
        originalRotation = rigTransform.rotation;

        if (descriptionTextUI != null)
            descriptionTextUI.text = ""; // Clear text initially
    }

    public void StartTourManually()
    {
        if (tourPoints.Length > 0)
        {
            StartCoroutine(RunTour());

            GameObject startButton = GameObject.Find("StartTourButton");
            if (startButton != null)
                startButton.SetActive(false);
        }
    }

    IEnumerator RunTour()
    {
        foreach (TourPoint point in tourPoints)
        {
            // --- HIDE TEXT while moving ---
            if (descriptionTextUI != null)
                descriptionTextUI.text = "";

            // Stop any playing audio
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();

            // Move and rotate
            yield return StartCoroutine(MoveToPosition(point.target.position, point.moveSpeed));
            yield return StartCoroutine(RotateToRotation(point.target.rotation, point.rotateSpeed));

            //  Play narration
            if (audioSource != null && point.voiceOverClip != null)
            {
                audioSource.clip = point.voiceOverClip;
                audioSource.Play();
            }

            //  Show description
            if (descriptionTextUI != null)
                descriptionTextUI.text = point.descriptionText;

            // Wait for audio OR stay time — whichever is longer
            float waitTime = point.stayTime;
            if (point.voiceOverClip != null)
                waitTime = Mathf.Max(waitTime, point.voiceOverClip.length);

            yield return new WaitForSeconds(waitTime);
        }

        Debug.Log("Tour complete. Returning to original position...");

        // Hide text while returning
        if (descriptionTextUI != null)
            descriptionTextUI.text = "";

        // Stop audio if still playing
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        // Move back to start
        rigTransform.position = originalPosition;
        rigTransform.rotation = originalRotation;

        Debug.Log("Returned to starting position.");
    }

    IEnumerator MoveToPosition(Vector3 targetPos, float speed)
    {
        while (Vector3.Distance(rigTransform.position, targetPos) > 0.01f)
        {
            rigTransform.position = Vector3.MoveTowards(
                rigTransform.position,
                targetPos,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }

    IEnumerator RotateToRotation(Quaternion targetRot, float speed)
    {
        while (Quaternion.Angle(rigTransform.rotation, targetRot) > 0.5f)
        {
            rigTransform.rotation = Quaternion.RotateTowards(
                rigTransform.rotation,
                targetRot,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }
}
