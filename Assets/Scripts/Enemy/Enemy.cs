using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private string unitName;
    [SerializeField] private float damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;
    [SerializeField] private float distance;
    [SerializeField] private float healAmount;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int level;
    [SerializeField] private float multiplier;
    
    private float currentHealth;
    
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

    private float AttackSpeed
    {
        get => attackSpeed;
        set
        {
            attackSpeed = value;
            _animator.SetFloat("AttackSpeed", attackSpeed);
        }
    }

    private void Awake()
    {
        CalculateLevelStats();
    }

    private void Start()
    {
        // If is not tower initialize NavMeshAgent and Animator components
        if (!isBase)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(playerBaseTransform.position);
            _agent.speed = moveSpeed;
            _animator = GetComponent<Animator>();
            AttackSpeed = attackSpeed;
        }

        currentHealth = health;
        slider.maxValue = health;
        slider.value = currentHealth;   // Slider maxHealth And CurrentHealth set;
        
        if (isMelee && isHealer)
            shootPosition = null;
    }

    private void Update()
    {
        if (isBase) return;

        HpBarLookCamera();
        
        if (!_targetLocated)
        {
            DistanceCheck();
        }
    }
    
    private void HpBarLookCamera()
    {
        var campos = Camera.main.transform.position;
        campos.x = hpSliderGo.transform.position.x;
        hpSliderGo.transform.LookAt(campos);
    }
    
    public void TakeDamage(float value)
    {
        hpSliderGo.gameObject.SetActive(true);
        
        if (!isBase)
            EventParticleManager.OnDamageParticleSpawn(this.transform);
        
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
        _animator.SetBool("isRunning", true);
        if (isHealer && hitColliders[0].CompareTag("NotHealable"))
        {
            _agent.speed = moveSpeed;
            _animator.SetBool("isAttacking", false);
            _animator.SetBool("isRunning", true);
            _targetLocated = false;
            return;
        }
        if (hitColliders.Length == 0)
        {
            _agent.speed = moveSpeed;
            _animator.SetBool("isAttacking", false);
            _animator.SetBool("isRunning", true);
            _targetLocated = false;
            return;
        }
       
        closestEnemy = hitColliders[0].gameObject;
        
        if (isHealer)
        {
            var closestScript = closestEnemy.GetComponent<Enemy>();                          //Check closest unity health value for decide heal unit or return;
            if (Math.Abs(closestScript.currentHealth - closestScript.health) < 0.8f) return; // Last value is a 0.5f Tolerance
        }
        
        transform.DOLookAt(closestEnemy.transform.position, 0.25f, AxisConstraint.Y);
        
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isAttacking", true);
        _agent.speed = 0;
        _targetLocated = true;
       
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
        EventParticleManager.OnAuraParticleSpawn(this.transform);
        
        _targetLocated = false;
    }
    
    private void UpdateHealth()
    {
        slider.DOValue(currentHealth, 0.5f);
    }

    public void TakeHeal(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth += value,0 , health);
        EventParticleManager.OnHealParticleSpawn(this.transform);
        UpdateHealth();
    }
    private void CalculateLevelStats()
    {
        damage += level * multiplier;
        moveSpeed += level * multiplier;
        health += level * multiplier;
        attackSpeed += level * multiplier;
        distance += level * multiplier;
    }
}