using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class HandController : MonoBehaviour
{
    private InputAction gripInputAction;
    private InputAction triggerInputAction;
    private InputAction movementYInputAction;
    private InputAction movementXInputAction;

    public InputActionAsset inputAction;

    public XROrigin origin;
    public Camera camera;

    public float moveSpeed;


    // Start is called before the first frame update
    void Start()
    {
        // initilize controller inputs
        gripInputAction = inputAction.FindActionMap("XRI RightHand Interaction").FindAction("Select Value");
        triggerInputAction = inputAction.FindActionMap("XRI RightHand Interaction").FindAction("Activate Value");
        movementYInputAction = inputAction.FindActionMap("XRI RightHand Interaction").FindAction("Translate Anchor");
        movementXInputAction = inputAction.FindActionMap("XRI RightHand Interaction").FindAction("Rotate Anchor");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // grip input read
        if(gripInputAction.ReadValue<float>() > 0.3f)
        {
        //       Debug.Log("grip");
         }

        // trigger input read
         if (triggerInputAction.ReadValue<float>() > 0.3f)
         {
         //      Debug.Log("trigger");
         }

        // forward movement
        origin.transform.position += camera.transform.forward.normalized * moveSpeed * Time.fixedDeltaTime * movementYInputAction.ReadValue<Vector2>().y;

        // sideways movement
        origin.transform.position += camera.transform.right.normalized * moveSpeed * Time.fixedDeltaTime * movementXInputAction.ReadValue<Vector2>().x;
    }
}
