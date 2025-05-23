using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentBehavior : MonoBehaviour
{
    public float upperTimeLimit;
    public float lowerTimeLimit;

    public float delayedTimeUpperLimit;
    public float delayedTimeLowerLimit;

    private int randomStudent;
    private int studentCounter;

    public float timer = 0;
    private bool timerReset = true;

    public StudentActions[] studentList;

    private TeacherBoard board;
    private MenuScript uiCanvas;

    private bool paused = false;
    private bool finalSentence = false;

    void Start()
    {
        board = GameObject.Find("GreetingBoard").GetComponent<TeacherBoard>();
        uiCanvas = GameObject.Find("UICanvas").GetComponent<MenuScript>();
        InitializeTimer();
    }

    void Update()
    {
        if (paused) return;

        var currentStudent = studentList[studentCounter];

        if (currentStudent.isInMeltdown)
        {
            Debug.Log("Meltdown in progress, waiting...");
            return;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (!timerReset && timer <= 0)
        {
            timerReset = true;

            if (currentStudent.PlayVoiceLine())
            {
                currentStudent.ResetEyeContact();

                if (studentCounter == studentList.Length - 1)
                {
                    studentCounter = 0;
                    board.StopWait();

                    if (finalSentence)
                    {
                        uiCanvas.SetRetryActive();
                        board.DisplayUI("Congratulations!" + '\n' + "You have reached the end of the training scenario");
                    }
                }
                else
                {
                    studentCounter++;
                    StartTimer();
                }
            }
            else
            {
                if (!currentStudent.isInMeltdown)
                {
                    board.DisplayUI("No eye contact with student " + (studentCounter + 1) +
                        '\n' + "Hint: Maintaining visual contact with students as they answer is essential");

                    currentStudent.ResetEyeContact();

                    if (studentCounter != studentList.Length - 1)
                    {
                        studentCounter++;
                        StartTimer();
                    }
                    else
                    {
                        studentCounter = 0;
                        board.StopWait();

                        if (finalSentence && uiCanvas.attempts > 0)
                        {
                            uiCanvas.SetRetryActive();
                            board.DisplayUI("Congratulations!" + '\n' + "You have reached the end of the training scenario");
                        }
                    }
                }
            }
        }
    }

    public void InitializeTimer()
    {
        studentCounter = 0;
        randomStudent = Random.Range(0, studentList.Length);

        foreach (StudentActions student in studentList)
        {
            student.ResetVoiceLines();
        }
    }

    public void StopTimer()
    {
        timer = 0;
        timerReset = true;
        finalSentence = false;
    }

    public void StartTimer()
    {
        var currentStudent = studentList[studentCounter];

        if (currentStudent.isInMeltdown)
        {
            Debug.Log("Skipping timer start — student is in meltdown.");
            return;
        }

        if (currentStudent.isVerbal || currentStudent.GetAnimatedResponse())
        {
            timerReset = false;

            if (studentCounter == randomStudent)
            {
                timer = Random.Range(delayedTimeLowerLimit, delayedTimeUpperLimit);
            }
            else
            {
                timer = Random.Range(lowerTimeLimit, upperTimeLimit);
            }

            currentStudent.StartEyeContact();
        }
        else
        {
            // Delay the non-verbal movement slightly to let meltdown logic run first
            StartCoroutine(DelayNonVerbalMovement(currentStudent));
        }

        Debug.Log("Timer set to: " + timer);
    }

    private IEnumerator DelayNonVerbalMovement(StudentActions student)
    {
        yield return new WaitForSeconds(4f); // Delay gives time for meltdown to trigger

        if (!student.isInMeltdown)
        {
            student.StartMoving();
        }
        else
        {
            Debug.Log("Non-verbal movement skipped — meltdown triggered during delay.");
        }
    }

    public void NonVerbalTimerStart()
    {
        timerReset = false;
        timer = Random.Range(lowerTimeLimit, upperTimeLimit);
        studentList[studentCounter].StartEyeContact();
    }

    public int getCurrentStudent()
    {
        return studentCounter;
    }

    public void FinalSentence()
    {
        finalSentence = true;
    }

    public void PauseTimer()
    {
        paused = true;
    }

    public void UnpauseTimer()
    {
        paused = false;
    }
}
