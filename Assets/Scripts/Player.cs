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
   
    [Header("Type Bools")]
    [SerializeField] private bool isRange;
    [SerializeField] private bool isMelee;
    [SerializeField] private bool isTower,isBase;
    
    [Header("References")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform enemyBaseTransform;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject hpSliderGo;
    [SerializeField] private Slider slider;
    
    private NavMeshAgent _agent;
    private Animator _animator;
    private GameObject _closestEnemy;
    
    private void Start()
    {
        if (!isTower)
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(enemyBaseTransform.position);
            _agent.speed = moveSpeed;
        }
        
        slider.maxValue = health;
        slider.value = currentHealth; // Slider maxHealth And CurrentHealth set;
        
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
        UpdateHealth();  // Update HealthBar slider method
    }

    private void Death()
    {
        if (isBase)
            GameManager.Instance.GameOver();
        
        Destroy(gameObject);
    }

    private void DistanceCheck()
    {
        // Check enemies given distance variable and choose closest one
        var hitColliders = Physics.OverlapSphere(transform.position, distance, enemyLayer);

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
    }

    private void Attack()  // ------ Calling from animation attack event ------ //
    {
        
        if (_closestEnemy == null) return;
        
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

    private void UpdateHealth()
    {
        slider.value = currentHealth;
    }
   
}
