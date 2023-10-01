using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Transform _player;
    public NavMeshAgent navMeshAgent;
    [SerializeField] private LayerMask _whatIsGround;
    [Header("Patrolling")]
    Vector3 walkPoint;
    private bool _walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    [SerializeField] private float _timeBetweenAttacks;
    [HideInInspector] public bool isAttacking = false;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public UnityEvent Attack;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerEntity>().transform;
        if (Attack == null) Attack = new();
    }

    private void OnEnable()
    {
        EnemyEntity enemyEntity = gameObject.GetComponent<EnemyEntity>();
        enemyEntity.OnDeath += StopAI;
    }

    private void OnDisable()
    {
        EnemyEntity enemyEntity = gameObject.GetComponent<EnemyEntity>();
        enemyEntity.OnDeath -= StopAI;
    }
    private void Update()
    {
        Collider[] sight = Physics.OverlapSphere(transform.position, sightRange);
        Collider[] attack = Physics.OverlapSphere(transform.position, attackRange);
        playerInSightRange = sight.Any(c => c.CompareTag("Player"));
        playerInAttackRange = attack.Any(c => c.CompareTag("Player"));

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }
    private void Patrolling()
    {
        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet) navMeshAgent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) _walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle * walkPointRange;
        walkPoint = new Vector3(randomPoint.x,transform.position.y,randomPoint.y);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, _whatIsGround))
            _walkPointSet = true;
    }
    private void ChasePlayer()
    {
        navMeshAgent.SetDestination(_player.position);
    }
    public void AttackPlayer()
    {
        navMeshAgent.SetDestination(transform.position);
        transform.LookAt(_player);

        if (!isAttacking)
        {
            Attack.Invoke();

            isAttacking = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
    }

    private void ResetAttack ()
    {
        isAttacking = false;
    }

    void StopAI()
    {

    }
}
