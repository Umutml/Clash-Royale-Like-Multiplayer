using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float currentHealth;
    [SerializeField] private float health;
    [SerializeField] private float distance;

    [Header("Type Bools")]
    [SerializeField] private bool isMelee;
    [SerializeField] private bool isRange;
    [SerializeField] private bool isTower;
    [SerializeField] private bool isBase;
    
    [Header("Referances")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform playerBaseTransform;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject hpSliderGo;
    [SerializeField] private Slider slider;
    
    [SerializeField]private GameObject closestEnemy;
    
    private bool _isAttacking;
    private bool _targetLocated;
    private NavMeshAgent _agent;
    private Animator _animator;
   
    private void Start()
    {
        if (!isTower)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(playerBaseTransform.position);
            _agent.speed = moveSpeed;
            _animator = GetComponent<Animator>();
        }
        
        slider.maxValue = health;
        slider.value = currentHealth;   // Slider maxHealth And CurrentHealth set;
        
        if (isMelee)
            shootPosition = null;
    }

    private void FixedUpdate()
    {
        if (isTower) return;
        DistanceCheck();
        hpSliderGo.transform.LookAt(Camera.main.transform.position);
    }

    public void TakeDamage(float value)
    {
        currentHealth -= value;
        if (currentHealth <= 0) Death();
        UpdateHealth();     // Update HealthBar slider method
    }

    private void Death()
    {
        if (isBase)
            GameManager.Instance.Win();
        
        Destroy(gameObject);
    }

    private void DistanceCheck()
    {
        // Check enemies given distance variable and choose closest one
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
            _animator.SetBool("isAttacking", false);
            _animator.SetBool("isRunning", true);
            _isAttacking = false;
            return;
        }

        closestEnemy = hitColliders[0].gameObject;
        _targetLocated = true;
        
        _agent.SetDestination(closestEnemy.transform.position);
        Vector3 tempCloses = closestEnemy.transform.position;
        tempCloses.y = transform.position.y;                    // Give destination and lookat closest just y axis
        transform.LookAt(tempCloses);
        
        
        var attackDist = Vector3.Distance(transform.position, closestEnemy.transform.position);
        if (attackDist-0.2f <= _agent.stoppingDistance && !_isAttacking)
        {
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isAttacking", true);
            _isAttacking = true;
            _agent.speed = 0;
        }
    }
    
    
    private void Attack()   // ------ Calling from animation attack event ------ //
    {
        if (closestEnemy == null) return;

        if (isMelee)
        {
            closestEnemy.GetComponent<Player>().TakeDamage(damage);
        }
        
        if (isRange)
        {
            // OBJECT POOLING FOR SPAWNED ARROWS
            var cloneArrow = Lean.Pool.LeanPool.Spawn(arrowPrefab, shootPosition.position, Quaternion.identity);
            cloneArrow.GetComponent<Arrow>().isPlayer = true;
            cloneArrow.transform.forward = closestEnemy.transform.position;
            cloneArrow.GetComponent<Arrow>().targetTransform = closestEnemy.transform;
            cloneArrow.GetComponent<Arrow>().arrowDamage = damage;
        }
    }
    
    private void UpdateHealth()
    {
        slider.value = currentHealth;
    }
}