using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuScript : MonoBehaviour
{
    public GameObject tutorialEndPanel;
    public GameObject teacherBoard;

    public GameObject player;
    private Vector3 position;
    private Quaternion rotation;

    public int attempts = 2;
    public GameObject retryButton;
    public GameObject continueButton;

    public void Start()
    {
        // find initial player position
        position = player.transform.position;
        rotation = player.transform.rotation;
    }

    public void Continue()
    {
        if(attempts == 0)
        {
            SetRetryActive();
        }
        else
        {
            attempts--;
        }

        tutorialEndPanel.SetActive(false);
        teacherBoard.GetComponent<TeacherBoard>().UnpauseTimer();
    }

    public void TryAgain()
    {
        // disable the UI
        tutorialEndPanel.SetActive(false);

        // reset the input sentence
        teacherBoard.GetComponent<TeacherBoard>().ResetSentence();

        // reset player position
        player.transform.position = position;
        player.transform.rotation = rotation;

        continueButton.SetActive(true);
        retryButton.SetActive(false);

        attempts = 2;
    }

    public void SetRetryActive()
    {
        continueButton.SetActive(false);
        retryButton.SetActive(true);
    }
}
