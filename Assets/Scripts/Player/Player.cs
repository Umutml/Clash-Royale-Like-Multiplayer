using System;
using System.Collections;
using DG.Tweening;
using Lean.Pool;
using Photon.Bolt;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : EntityBehaviour<ICharacters>
{
    public ScriptableSettings scrData;
    
    [Header("References")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform enemyBaseTransform;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject hpSliderGo;
    [SerializeField] private Slider slider;
    
    private float currentHealth;
    private NavMeshAgent _agent;
    private Animator _animator;
    private GameObject _closestEnemy;
    [SerializeField]private bool targetLocated;

    private float AttackSpeed
    {
        get => scrData.attackSpeed;
        set
        {
            scrData.attackSpeed = value;
            _animator.SetFloat("AttackSpeed", scrData.attackSpeed);
        }
    }
    
    public override void Attached()
    {
        
    }

    public override void SimulateOwner()
    {
        if (currentHealth <= 0) Death();
    }
    
    private void Awake()
    {
        //CalculateLevelStats();
        currentHealth = scrData.health;
        slider.maxValue = scrData.health;
        slider.value = currentHealth; // Slider maxHealth And CurrentHealth set;
    }
    private void Start()
    {
        if (!scrData.isBase)
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(enemyBaseTransform.position);
            _agent.speed = scrData.moveSpeed;
            AttackSpeed = scrData.attackSpeed;
        }
        if (scrData.isMelee && scrData.isHealer)
            shootPosition = null;
    }
    

    private void Update()
    {
        if (scrData.isBase) return;
        
        HpBarLookCamera();
        if (!targetLocated && _agent.isActiveAndEnabled)
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
        
        if (!scrData.isBase)
            EventParticleManager.OnDamageParticleSpawn(this.transform);
        UpdateHealth(); 
         
        // Update HealthBar slider method
    }

    private void Death()
    {
        if (scrData.isBase)
            GameManager.Instance.GameOver();

        GameManager.Instance.EnemyKill++;
        BoltNetwork.Destroy(gameObject);
            
    }  
    private void DistanceCheck()
    {
        // Check enemies given distance variable and choose closest one
        var hitColliders = Physics.OverlapSphere(transform.position, scrData.distance, targetLayer);
        _animator.SetBool("isRunning", true);
        if (scrData.isHealer && hitColliders[0].CompareTag("NotHealable"))
        {
            _agent.speed = scrData.moveSpeed;
            _animator.SetBool("isAttacking", false);
            targetLocated = false;
            _animator.SetBool("isRunning", true);
            return;
        }
        
        if (hitColliders.Length == 0)
        {
            _agent.speed = scrData.moveSpeed;
            _animator.SetBool("isAttacking", false);
            targetLocated = false;
            _animator.SetBool("isRunning", true);
            return;
        }

        _closestEnemy = hitColliders[0].gameObject;
        
        if (scrData.isHealer)
        {
            var closestScript = _closestEnemy.GetComponent<Player>();                          //Check closest unity health value for decide heal unit or return;
            if (Math.Abs(closestScript.currentHealth - closestScript.scrData.health) < 0.8f) return; // Last value is a 0.5f Tolerance
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
        
        if (scrData.isRange)
        {
            // OBJECT POOLING FOR SPAWNED ARROWS
            //var cloneArrow = LeanPool.Spawn(arrowPrefab, shootPosition.position, Quaternion.identity); 
            var cloneArrow = Instantiate(arrowPrefab, shootPosition.position, Quaternion.identity);
            cloneArrow.GetComponent<Arrow>().isPlayer = false;
            cloneArrow.transform.forward = _closestEnemy.transform.position;
            cloneArrow.GetComponent<Arrow>().targetTransform = _closestEnemy.transform;
            cloneArrow.GetComponent<Arrow>().arrowDamage = scrData.damage;
        }
        if (scrData.isMelee)
        {
            _closestEnemy.GetComponent<Enemy>().TakeDamage(scrData.damage);
        }
    }

    private void Heal()
    {
        if (_closestEnemy != null)
        {
            _closestEnemy.GetComponent<Player>().TakeHeal(scrData.healAmount);
        }
        EventParticleManager.OnAuraParticleSpawn(this.transform);
        targetLocated = false;
    }

    private void UpdateHealth()
    {
        slider.DOValue(currentHealth, 0.25f);
    }
    
    public void TakeHeal(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth += value,0 , scrData.health);
        EventParticleManager.OnHealParticleSpawn(this.transform);
        UpdateHealth();
    }

   // private void CalculateLevelStats()
   // {
    //    scrData.damage += scrData.level * scrData.multiplier;
    //    scrData.moveSpeed += scrData.level * scrData.multiplier;
    //    scrData.health += scrData.level * scrData.multiplier;
    //    scrData.attackSpeed += scrData.level * scrData.multiplier;
    //    scrData.distance += scrData.level * scrData.multiplier;
    //}
   
}
