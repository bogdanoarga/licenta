using UnityEngine;

public class MeltdownPictogram : MonoBehaviour
{
    [SerializeField] private StudentMeltdown targetStudent;

    public void OnPictogramPressed()
    {
        if (targetStudent != null)
        {
            // Set the confirmation flag before continuing
            targetStudent.animator.SetBool("meltdownConfirmed", true);
            targetStudent.SendToQuietPlace();

        }
        else
        {
            Debug.LogWarning("MeltdownPictogram: No student assigned!");
        }
    }
}
