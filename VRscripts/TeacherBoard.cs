using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TeacherBoard : MonoBehaviour
{
    public GameObject UIPanel;
    public TextMeshProUGUI displayText;

    private AudioManager audioManager;
    private StudentBehavior studentBehavior;

    private static string formedSentence = "";
    public static string[] targetSentence = { "Buna dimineata!",
                                             "Cum te simti azi?"};
    //"Cum ai dormit azi noapte",
    // "Te-ai trezit rau sau bine dimineata"};

    private int sentenceCount = 0;
    private bool waitForResponse = false;

    public void Start()
    {
        // instantiate object references
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        studentBehavior = GameObject.Find("StudentManager").GetComponent<StudentBehavior>();
    }

    public void SelectedCube(string value)
    {
        // add word to current sentence
        formedSentence += value;
        audioManager.Play(value);

        // check for student answer
        if (!waitForResponse)
        {
            // wrong sentence input check
            if (!targetSentence[sentenceCount].Contains(formedSentence))
            {
                DisplayUI("Wrong sentence order" + '\n' + "Hint: Follow the order of the images on the board");
                formedSentence = "";
            }
            // correct sentence input check
            else if (formedSentence.Equals(targetSentence[sentenceCount]))
            {
                sentenceCount++;
                formedSentence = "";

                // check for final sentence
                if (sentenceCount == targetSentence.Length)
                {
                    studentBehavior.FinalSentence();
                }

                // start student actions
                WaitForAnswer();
                studentBehavior.StartTimer();
            }
        }
        else
        {
            DisplayUI("Hint: Wait for student " + (studentBehavior.getCurrentStudent() + 1) + " to answer before moving to the next sentence");
        }
    }

    public void ResetSentence()
    {
        formedSentence = "";
        sentenceCount = 0;

        waitForResponse = false;
        studentBehavior.InitializeTimer();
    }

    public void WaitForAnswer()
    {
        waitForResponse = true;
    }

    public void StopWait()
    {
        waitForResponse = false;
    }

    public void DisplayUI(string message)
    {
        studentBehavior.StopTimer();

        UIPanel.SetActive(true);
        displayText.text = message;

        formedSentence = "";

        studentBehavior.PauseTimer();
    }

    public void UnpauseTimer()
    {
        studentBehavior.UnpauseTimer();
    }
}
