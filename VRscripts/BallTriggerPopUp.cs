using UnityEngine;

public class BallTriggerPopUp : MonoBehaviour
{
    public GameObject popupCanvas; // assign in Inspector
    private bool hasShown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasShown && other.CompareTag("Player")) // safer and tag-based
        {
            popupCanvas.SetActive(true);
            hasShown = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popupCanvas.SetActive(false);
            hasShown = false;
        }
    }
}
