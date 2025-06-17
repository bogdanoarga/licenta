using UnityEngine;
using TMPro;

public class MeltdownPopupManager : MonoBehaviour
{
    public static MeltdownPopupManager Instance;

    [SerializeField] private GameObject popupObject;
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private float displayDuration = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        popupObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        popupText.text = message;
        popupObject.SetActive(true);
        CancelInvoke(nameof(HidePopup));
        Invoke(nameof(HidePopup), displayDuration);
    }

    private void HidePopup()
    {
        popupObject.SetActive(false);
    }
}
