using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricMovementWithVelocity : MonoBehaviour
{
    public float movementSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Vector3 forward, right;

    // prosty movement, mo¿na by dodaæ jakieœ velocity malej¹ce z czasem ¿eby nie zatrzymywa³ siê od razu

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

        moveVelocity = (forward * moveInput.z + right * moveInput.x) * movementSpeed;
    }

    void FixedUpdate()
    {
        rb.velocity = moveVelocity;
    }
}
