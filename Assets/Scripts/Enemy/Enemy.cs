using System;
using DG.Tweening;
using Photon.Bolt;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : EntityBehaviour<ICharacters>
{

    public ScriptableSettings scrData;
   
    [Header("References")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform playerBaseTransform;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject hpSliderGo;
    [SerializeField] private Slider slider;
    
    private GameObject closestEnemy;
    private float currentHealth;
    private bool _isAttacking;
    private bool _targetLocated;
    private NavMeshAgent _agent;
    private Animator _animator;

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
        // If is not tower initialize NavMeshAgent and Animator components
        if (!scrData.isBase)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(playerBaseTransform.position);
            _agent.speed = scrData.moveSpeed;
            _animator = GetComponent<Animator>();
            AttackSpeed = scrData.attackSpeed;
        }
        
        currentHealth = scrData.health;
        slider.maxValue = scrData.health;
        slider.value = currentHealth;   // Slider maxHealth And CurrentHealth set;
        
        if (scrData.isMelee && scrData.isHealer)
            shootPosition = null;
    }

    private void Update()
    {
        if (scrData.isBase) return;
        
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
        
        if (!scrData.isBase)
            EventParticleManager.OnDamageParticleSpawn(this.transform);
        
        currentHealth -= value;
        UpdateHealth(); 
           // Update HealthBar slider method
    }

    private void Death()
    {
        if (scrData.isBase)
            GameManager.Instance.Win();
        GameManager.Instance.PlayerKill++;
        
            BoltNetwork.Destroy(this.gameObject);
            EventParticleManager.OnDeathParticleSpawn(this.transform);
        
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
            _animator.SetBool("isRunning", true);
            _targetLocated = false;
            return;
        }
        if (hitColliders.Length == 0)
        {
            _agent.speed = scrData.moveSpeed;
            _animator.SetBool("isAttacking", false);
            _animator.SetBool("isRunning", true);
            _targetLocated = false;
            return;
        }
       
        closestEnemy = hitColliders[0].gameObject;
        
        if (scrData.isHealer)
        {
            var closestScript = closestEnemy.GetComponent<Enemy>();                          //Check closest unity health value for decide heal unit or return;
            if (Math.Abs(closestScript.currentHealth - closestScript.scrData.health) < 0.8f) return; // Last value is a 0.5f Tolerance
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
        
        if (scrData.isRange)
        {
            // OBJECT POOLING FOR SPAWNED ARROWS
            //var cloneArrow = Lean.Pool.LeanPool.Spawn(arrowPrefab, shootPosition.position, Quaternion.identity);
            var cloneArrow = Instantiate(arrowPrefab, shootPosition.position, Quaternion.identity);
            cloneArrow.GetComponent<Arrow>().isPlayer = true;
            cloneArrow.transform.forward = closestEnemy.transform.position;
            cloneArrow.GetComponent<Arrow>().targetTransform = closestEnemy.transform;
            cloneArrow.GetComponent<Arrow>().arrowDamage = scrData.damage;
        }
        
        if (scrData.isMelee)
        {
            closestEnemy.GetComponent<Player>().TakeDamage(scrData.damage);
        }
    }
    
    private void Heal()
    {
        if (closestEnemy != null)
            closestEnemy.GetComponent<Enemy>().TakeHeal(scrData.healAmount);
        EventParticleManager.OnAuraParticleSpawn(this.transform);
        
        _targetLocated = false;
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
    //private void CalculateLevelStats()
    //{
        //scrData.damage = scrData.damage + scrData.level * scrData.multiplier;
       // scrData.moveSpeed = scrData.level * scrData.multiplier;
       // scrData.health = scrData.level * scrData.multiplier;
        //scrData.attackSpeed = scrData.level * scrData.multiplier;
        //scrData.distance = scrData.level * scrData.multiplier;
}
