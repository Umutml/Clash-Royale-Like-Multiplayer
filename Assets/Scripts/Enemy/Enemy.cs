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
    [SerializeField] private float healAmount;
    
    [Header("Type Bools")]
    [SerializeField] private bool isMelee;
    [SerializeField] private bool isRange;
    [SerializeField] private bool isBase;
    [SerializeField] private bool isHealer;
    
    [Header("References")]
    [SerializeField] private LayerMask targetLayer;
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
        // If is not tower initialize NavMeshAgent and Animator components
        if (!isBase)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(playerBaseTransform.position);
            _agent.speed = moveSpeed;
            _animator = GetComponent<Animator>();
        }
        
        slider.maxValue = health;
        slider.value = currentHealth;   // Slider maxHealth And CurrentHealth set;
        
        if (isMelee && isHealer)
            shootPosition = null;
    }

    private void Update()
    {
        hpSliderGo.transform.LookAt(Camera.main.transform.position);
        if (isBase) return;
        if (!_targetLocated)
        {
            DistanceCheck();
        }
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
        GameManager.Instance.PlayerKill++;
        Destroy(gameObject);
    }

    private void DistanceCheck()
    {
        // Check enemies given distance variable and choose closest one
        var hitColliders = Physics.OverlapSphere(transform.position, distance, targetLayer);
        
        if (isHealer && hitColliders[0].CompareTag("NotHealable"))
        {
            _agent.speed = moveSpeed;
            _animator.SetBool("isAttacking", false);
            _animator.SetBool("isRunning", true);
            return;
        }
        if (hitColliders.Length == 0)
        {
            _agent.speed = moveSpeed;
            _animator.SetBool("isAttacking", false);
            _animator.SetBool("isRunning", true);
            return;
        }
       
        closestEnemy = hitColliders[0].gameObject;
        Vector3 tempCloses = closestEnemy.transform.position;
        tempCloses.y = transform.position.y;                    // Give destination and lookat closest just y axis
        transform.LookAt(tempCloses);
        
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isAttacking", true);
        _agent.speed = 0;
        _targetLocated = true;

        #region DifferentCalculate

        /*if (hitColliders.Length == 0)
      {
          // If no enemies are detected, move towards player base
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
      }*/

        
        //var attackDist = Vector3.Distance(transform.position, closestPos);
        //attackDist -= 0.5f; // Create attack gap for stopping distance
        /*if (attackDist <= _agent.stoppingDistance && !_isAttacking)
        {
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isAttacking", true);
            _isAttacking = true;
            _agent.speed = 0;
        }*/

        #endregion
    }
    
    private void Attack()   // ------ Calling from animation attack event ------ //
    {
        if (closestEnemy == null)
        {
            _targetLocated = false;
            return;
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
        
        if (isMelee)
        {
            closestEnemy.GetComponent<Player>().TakeDamage(damage);
        }
    }
    
    private void Heal()
    {
        if (closestEnemy != null)
            closestEnemy.GetComponent<Enemy>().TakeHeal(healAmount);
        _targetLocated = false;
    }
    
    private void UpdateHealth()
    {
        slider.DOValue(currentHealth, 0.5f);
    }

    public void TakeHeal(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth += value,0 , health);
        UpdateHealth();
        // Play heal particle transform position;
    }
}