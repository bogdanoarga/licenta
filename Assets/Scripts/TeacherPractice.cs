using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

// VARIANTA 1

public class TeacherPractice : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float rotationSpeed;
    private bool isGameOverActive = false;
    private float raiseHandTimer;
    [SerializeField] private float raiseHandTimeLimit = 10f;

    private Vector3 moveDirection;
    private Vector3 velocity;
    public GameObject GameOverMenu;

    private float levelcompletedtimer;
    [SerializeField] private float levelcompletedtimerlimit = 5f;


    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private float maxFallSpeed; // Added maximum falling speed

    private CharacterController controller;
    private Animator anim;
    //public Canvas canvas;
    // public GameObject canva1 = GameObject.FindWithTag("CanvasMenu");
    //public GameObject overmenu1 = GameObject.FindWithTag("OverMenu");
    public static GameObject random;
    public static TextMeshProUGUI hinttext;
    public static GameObject child;

    private bool correctraisehand=false;
    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        GameOverMenu.SetActive(false);
        random = this.GameOverMenu;
        child = random.transform.GetChild(4).gameObject;
        hinttext = child.GetComponent<TextMeshProUGUI>();
        Debug.Log(hinttext);


    } 

    public void EnableGameOverMenu()
    {
        GameOverMenu.SetActive(true);
        moveDirection = Vector3.zero;
        velocity = Vector3.zero;
        isGameOverActive = true;
        Idle();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Debug.Log(transform.position.z);

        if (isGameOverActive==false)
        {
            if (transform.position.z > 30f && Input.GetKeyDown(KeyCode.Space))
            {
                correctraisehand = true;
            }
            if (transform.position.z > 30f && !correctraisehand) // dupa tabla si nu ridica mana 
            {
                raiseHandTimer += Time.deltaTime;
                if (raiseHandTimer >= raiseHandTimeLimit)
                {
                    EnableGameOverMenu();
                    isGameOverActive = true;
                    hinttext.text = "HINT: You have to raise you hand in order to point to the picture!";
                }
            }

            else
            {
                if (transform.position.z <= 30f && Input.GetKey(KeyCode.Space) && correctraisehand == false) // inainte de tabla si ridica manna nu i voie
                {

                    EnableGameOverMenu();
                    hinttext.text = "HINT: You have to be near the picture board in order to raise hand!";
                    Debug.Log(hinttext.text);
                }

                else
                {
                    
                    if (transform.position.z > 30f && correctraisehand == true) // dupa tabla si si ridica mana, FB
                    {
                        levelcompletedtimer += Time.deltaTime;
                        if (levelcompletedtimer >= levelcompletedtimerlimit)
                        {
                            EnableGameOverMenu();
                            isGameOverActive = true;
                            hinttext.text = "WELL DONE! You succesfully completed the task!";
                            // level completed
                        }

                    }
                    raiseHandTimer = 0f;
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.Space) )
        {
                RaisingHand();
        }
    }

    private void Rotate()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -6.8f;
        }

        if (isGrounded)
        {
/*
            if (transform.position.z <= 34f && Input.GetKey(KeyCode.Space))
            {
                EnableGameOverMenu();
                hinttext.text = "asta i hintu pls ";
                Debug.Log(hinttext.text);
            }*/
            //else if (transform.position.z >= 34f && !Input.GetKey(KeyCode.Space))
            // {
            //        EnableGameOverMenu();
            //        hinttext.text = "asta i altuuuuuuu hintu pls ";
            // }
                
         

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            moveDirection = new Vector3(moveX, 0, moveZ);
            moveDirection = transform.TransformDirection(moveDirection);

            if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
            {
                Walking();
                Rotate();
            }
            else if (moveDirection == Vector3.zero)
            {
                Idle();
            }

            moveDirection *= moveSpeed;
        }

        controller.Move(moveDirection * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, maxFallSpeed); // Apply maximum falling speed

        controller.Move(velocity * Time.deltaTime);
    }


    private void Idle()
    {
        anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }

    private void Walking()
    {  
            moveSpeed = walkSpeed;
            anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
     
    }

    private void RaisingHand()
    {
        if (!isGameOverActive)
        {
            anim.SetTrigger("RaisingHand");
        }
    }
}


