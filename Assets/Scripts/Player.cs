using DG.Tweening;
using Lean.Pool;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float currentHealth;
    [SerializeField] private float health;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float distance;
    [SerializeField] private Transform enemyBaseTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private bool isRange;
    [SerializeField] private bool isMelee;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float damage;
    private NavMeshAgent _agent;

    private GameObject _closestEnemy;

    private bool isTargetLocated;
    [SerializeField]private bool isTower,isBase;

    [SerializeField] private GameObject hpSliderGo;
    [SerializeField]private Slider slider;
    
    private void Start()
    {
        if (!isTower)
        {
            animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(enemyBaseTransform.position);
            _agent.speed = moveSpeed;
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
        if (isBase)
            GameManager.Instance.GameOver();
        
        Destroy(gameObject);
    }

    private void DistanceCheck()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, distance, enemyLayer);

        if (hitColliders.Length == 0)
        {
            _agent.speed = moveSpeed;
            animator.SetBool("isAttacking", false);
            animator.SetBool("isRunning", true);
            return;
        }

        _closestEnemy = hitColliders[0].gameObject;
        Vector3 tempCloses = _closestEnemy.transform.position;
        tempCloses.y = transform.position.y;
        transform.LookAt(tempCloses);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", true);
        _agent.speed = 0;
    }

    private void Attack()
    {
        if (_closestEnemy == null) return;
        // OBJECT POOLING
        if (isRange)
        {
            var cloneArrow = LeanPool.Spawn(arrowPrefab, shootPosition.position, Quaternion.identity);
            cloneArrow.GetComponent<Arrow>().isPlayer = false;
            cloneArrow.transform.forward = _closestEnemy.transform.position;
            cloneArrow.GetComponent<Arrow>().targetTransform = _closestEnemy.transform;
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
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, distance);
    }
}
