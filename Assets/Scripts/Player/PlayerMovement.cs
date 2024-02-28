using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Vector3 forward, right;
    private bool isSprinting = false;

    public VitalStatsHandler vitalStatsHandler;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        forward = Camera.main.transform.forward;
        right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();
    }

    void Update()
    {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveInput.Normalize();

        RotatePlayer();
        Sprint(10f);

        float currentSpeed = isSprinting ? movementSpeed * sprintMultiplier : movementSpeed;
        moveVelocity = (forward * moveInput.z + right * moveInput.x) * currentSpeed;
    }

    void FixedUpdate()
    {
        rb.velocity = moveVelocity;
    }

    private void RotatePlayer()
    {
        if (moveInput.x != 0 || moveInput.z != 0)
            RotateTowards();
        else
            RotateTowardsMouse();
    }
    private void RotateTowards()
    {
        Vector3 directionToLook = (forward * moveInput.z + right * moveInput.x).normalized;
        if (directionToLook != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 1.1f * Time.deltaTime * 100);
        }
    }

    private void RotateTowardsMouse()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(cameraRay, out float rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Vector3 directionToLook = (pointToLook - transform.position).normalized;
            directionToLook.y = 0;
            if (directionToLook != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 1.1f * Time.deltaTime * 100);
            }
        }
    }

    private void Sprint(float staminaConsume)
    {
        if (Input.GetKey(KeyCode.LeftShift) && vitalStatsHandler.GetCurrentStamina() > 0f && (moveInput.x != 0 || moveInput.z != 0))
        {
            vitalStatsHandler.isUsingStamina = true;
            vitalStatsHandler.ConsumeStamina(10f);
            isSprinting = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && vitalStatsHandler.GetCurrentStamina() == 0f)
        {
            vitalStatsHandler.isUsingStamina = true;
            isSprinting = false;
        }
        else
        {
            vitalStatsHandler.isUsingStamina = false;
            isSprinting = false;
        }
    }
}

