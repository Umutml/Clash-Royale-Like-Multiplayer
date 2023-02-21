using System;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Ability Settings")] 
    [SerializeField] private string unitName;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float distance;
    [SerializeField] private float healAmount;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float multiplier;
    [SerializeField] private int level;
    
    private float currentHealth;
    
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
    [SerializeField]private bool targetLocated;


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
        currentHealth = health;
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
            AttackSpeed = attackSpeed;
        }
        if (isMelee && isHealer)
            shootPosition = null;
    }

    private void Update()
    {
        if (isBase) return;
        
        HpBarLookCamera();

        if (!targetLocated)
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
        currentHealth -= value;
        
        if (!isBase)
            EventParticleManager.OnDamageParticleSpawn(this.transform);
        
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
        _animator.SetBool("isRunning", true);
        if (isHealer && hitColliders[0].CompareTag("NotHealable"))
        {
            _agent.speed = moveSpeed;
            _animator.SetBool("isAttacking", false);
            targetLocated = false;
            _animator.SetBool("isRunning", true);
            return;
        }
        
        if (hitColliders.Length == 0)
        {
            _agent.speed = moveSpeed;
            _animator.SetBool("isAttacking", false);
            targetLocated = false;
            _animator.SetBool("isRunning", true);
            return;
        }

        _closestEnemy = hitColliders[0].gameObject;
        
        if (isHealer)
        {
            var closestScript = _closestEnemy.GetComponent<Player>();                          //Check closest unity health value for decide heal unit or return;
            if (Math.Abs(closestScript.currentHealth - closestScript.health) < 0.8f) return; // Last value is a 0.5f Tolerance
        }
        
        transform.DOLookAt(_closestEnemy.transform.position, 0.25f, AxisConstraint.Y);
        
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
        {
            _closestEnemy.GetComponent<Player>().TakeHeal(healAmount);
        }
        EventParticleManager.OnAuraParticleSpawn(this.transform);
        targetLocated = false;
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
