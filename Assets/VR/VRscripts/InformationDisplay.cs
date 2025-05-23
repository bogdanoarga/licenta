using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationDisplay : MonoBehaviour
{
    public string infoContent;
    public GameObject infoContainer;

    private bool displayed = false;

    public void UpdateToolTip()
    {
        if (!displayed)
        {   
            displayed = true;
            infoContainer.SetActive(true);
            infoContainer.GetComponentInChildren<TextMeshProUGUI>().text = infoContent;
        }
    }
}
