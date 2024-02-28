using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    private bool alreadyAttacked;

    public float sightRange, attackRange;
    [Range(0f, 360f)]
    public float sightAngle;
    public bool playerInSightRange, playerInAttackRange;

    private VitalStatsHandler vitalStatsHandler;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        vitalStatsHandler = GameObject.Find("VitalStatsHandler").GetComponent<VitalStatsHandler>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        bool playerInDistanceForSight = Vector3.Distance(transform.position, player.position) <= sightRange;
        bool playerInDistanceForAttack = Vector3.Distance(transform.position, player.position) <= attackRange;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        playerInSightRange = playerInDistanceForSight && angleToPlayer <= sightAngle / 2;
        playerInAttackRange = playerInDistanceForAttack;

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2F, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            // Tutaj dodaæ wykonanie ataku, mo¿e jakiœ generyk albo po prostu daæ 2 typy range i combat czy coœ
            vitalStatsHandler.TakeDamage(5);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    // podgl¹d zasiêgu
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;

        float totalFOV = sightAngle;
        float rayRange = sightRange;
        float halfFOV = totalFOV / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);

        int steps = 10;
        for (int i = 0; i <= steps; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(-halfFOV + (totalFOV / steps) * i, Vector3.up);
            Vector3 direction = rotation * transform.forward;
            Gizmos.DrawRay(transform.position, direction * rayRange);
        }
    }

}
