using System;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float currentHealth;
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float distance;
    [SerializeField] private float healAmount;
    
    [Header("Type Bools")]
    [SerializeField] private bool isRange;
    [SerializeField] private bool isMelee;
    [SerializeField] private bool isBase;
    [SerializeField] private bool isHealer;
    
    
    [Header("References")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform enemyBaseTransform;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject hpSliderGo;
    [SerializeField] private Slider slider;
    
    private NavMeshAgent _agent;
    private Animator _animator;
    [SerializeField]private GameObject _closestEnemy;
    private bool targetLocated;


    private void Awake()
    {
        slider.maxValue = health;
        slider.value = currentHealth; // Slider maxHealth And CurrentHealth set;
    }

    private void Start()
    {
        if (!isBase)
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(enemyBaseTransform.position);
            _agent.speed = moveSpeed;
        }
        if (isMelee && isHealer)
            shootPosition = null;
    }

    private void Update()
    {
        hpSliderGo.transform.LookAt(Camera.main.transform.position);
        if (isBase) return;
        if (!targetLocated)
        {
            DistanceCheck();
        }
        
    }

    public void TakeDamage(float value)
    {
        currentHealth -= value;
        if (currentHealth <= 0) Death();
        UpdateHealth();  // Update HealthBar slider method
    }

    private void Death()
    {
        if (isBase)
            GameManager.Instance.GameOver();

        GameManager.Instance.EnemyKill++;
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

        _closestEnemy = hitColliders[0].gameObject;
        Vector3 tempCloses = _closestEnemy.transform.position;  // Give destination and look at closest just y axis
        tempCloses.y = transform.position.y;                
        transform.LookAt(tempCloses);
        
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isAttacking", true);
        
        _agent.speed = 0;
        targetLocated = true;
    }

    private void Attack()  // ------ Calling from animation attack event ------ //
    {
        
        if (_closestEnemy == null)
        {
            targetLocated = false;
            return;
        }
        
        if (isRange)
        {
            // OBJECT POOLING FOR SPAWNED ARROWS
            var cloneArrow = LeanPool.Spawn(arrowPrefab, shootPosition.position, Quaternion.identity); 
            cloneArrow.GetComponent<Arrow>().isPlayer = false;
            cloneArrow.transform.forward = _closestEnemy.transform.position;
            cloneArrow.GetComponent<Arrow>().targetTransform = _closestEnemy.transform;
            cloneArrow.GetComponent<Arrow>().arrowDamage = damage;
        }
        if (isMelee)
        {
            _closestEnemy.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    private void Heal()
    {
        if (_closestEnemy != null)
            _closestEnemy.GetComponent<Player>().TakeHeal(healAmount);
        targetLocated = false;
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
