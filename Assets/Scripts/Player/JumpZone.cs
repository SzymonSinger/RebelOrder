using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class JumpZone : MonoBehaviour
{
    #region OldCode

    /*public Transform jumpDestination;
    public float jumpHeight;
    public float jumpSpeed;
    private bool moveToEnd = false;
    private Vector3 middlePosition;
    private Rigidbody playerRigibody;

    void Update()
    {
        HeightCalculation();

        if (jumpDestination != null)
        {
            if (moveToEnd)
            {
                MoveTowardsPosition(jumpDestination.position);
            }
        }
        else
        {
            Debug.LogWarning("Target object is not assigned.");
        }
    }

    private void HeightCalculation()
    {
        Vector3 directionToTarget = jumpDestination.position - transform.position;

        float distanceToTarget = (directionToTarget.magnitude);

        middlePosition = transform.position + directionToTarget * 0.5f;

        middlePosition.y += jumpHeight;
    }

    private void MoveTowardsPosition(Vector3 position)
    {
        Vector3 direction = position - transform.position;

        direction.Normalize();

        Vector3 movement = direction * jumpSpeed * Time.deltaTime;

        playerRigibody.MovePosition(transform.position + movement);

        if (Vector3.Distance(transform.position, position) < 0.1f)
        {
            moveToEnd = !moveToEnd;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRigibody = other.GetComponent<Rigidbody>();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MoveTowardsPosition(middlePosition);
            }
        }
    }*/

    #endregion

    #region NewCode
    public Transform jumpDestination;
    public float jumpHeight;
    public float jumpSpeed;
    private float gravityMultiplier = 2.0f;
    private bool isPlayerInside;
    private Rigidbody playerRigidbody;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRigidbody = other.GetComponent<Rigidbody>();
            isPlayerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Jump());
        }
    }

    IEnumerator Jump()
    {
        Vector3 startPosition = playerRigidbody.position;
        Vector3 targetPosition = jumpDestination.position;

        float startTime = Time.time;

        while (Time.time - startTime < 1.0f / jumpSpeed)
        {
            float t = (Time.time - startTime) * jumpSpeed;
            float height = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI) * jumpHeight;

            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);
            newPosition.y += height;

            playerRigidbody.MovePosition(newPosition);

            // Check if the player is above the ground
            if (Physics.Raycast(playerRigidbody.position, Vector3.down, out RaycastHit hit, 0.1f))
            {
                // Apply gravity to simulate a faster fall
                float fallSpeed = Mathf.Sqrt(2 * gravityMultiplier * jumpHeight);
                playerRigidbody.MovePosition(playerRigidbody.position + Vector3.down * fallSpeed * Time.deltaTime);
            }

            yield return null;
        }

        isPlayerInside = false;
    }
    

    #endregion
}
