using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float currentHealth;
    [SerializeField] private float health;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float distance;
    [SerializeField] private Transform playerBaseTransform;
    private bool _targetLocated;
    private NavMeshAgent _agent;
    [SerializeField] private GameObject arrowPrefab;

    [SerializeField]private Animator animator;
    
    [SerializeField]private GameObject closestEnemy;
    
    private bool _isAttacking;
    [SerializeField]private bool isMelee;
    [SerializeField]private bool isRange;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private bool isTower;
    
    [SerializeField] private GameObject hpSliderGo;
    [SerializeField] private Slider slider;

    private void Start()
    {
        if (!isTower)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(playerBaseTransform.position);
            _agent.speed = moveSpeed;
            animator = GetComponent<Animator>();
        }
        
        slider.maxValue = health;
        slider.value = currentHealth;
        
        if (isMelee)
        {
            shootPosition = null;
        }
    }

    private void Update()
    {
        if (isTower) return;
        DistanceCheck();
        hpSliderGo.transform.LookAt(Camera.main.transform.position);
    }

    public void TakeDamage(float value)
    {
        currentHealth -= value;
        if (currentHealth <= 0) Death();
        UpdateHealth();
    }

    private void Death()
    {
        Destroy(gameObject);
    }

    private void DistanceCheck()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, distance, playerLayer);

        if (hitColliders.Length == 0)
        {
            if (_targetLocated)
            {
                _agent.speed = moveSpeed;
                _targetLocated = false;
                closestEnemy = null;
            }
            _agent.speed = moveSpeed;
            _agent.SetDestination(playerBaseTransform.position);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isRunning", true);
            _isAttacking = false;
            return;
        }

        closestEnemy = hitColliders[0].gameObject;
        _targetLocated = true;
        _agent.SetDestination(closestEnemy.transform.position);
        Vector3 tempCloses = closestEnemy.transform.position;
        tempCloses.y = transform.position.y;
        transform.LookAt(tempCloses);
        var attackDist = Vector3.Distance(transform.position, closestEnemy.transform.position);
        if (attackDist-0.2f <= _agent.stoppingDistance && !_isAttacking)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", true);
            _isAttacking = true;
            _agent.speed = 0;
        }
        
    }

    
    
    private void Attack()
    {
        if (closestEnemy == null) return;

        if (isMelee)
        {
            closestEnemy.GetComponent<Player>().TakeDamage(damage);
        }
        
        if (isRange)
        {
            var cloneArrow = Lean.Pool.LeanPool.Spawn(arrowPrefab, shootPosition.position, Quaternion.identity);
            cloneArrow.GetComponent<Arrow>().isPlayer = true;
            cloneArrow.transform.forward = closestEnemy.transform.position;
            cloneArrow.GetComponent<Arrow>().targetTransform = closestEnemy.transform;
            
        }
    }
    
    private void UpdateHealth()
    {
        slider.value = currentHealth;
    }
}