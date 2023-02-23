using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float arrowDamage;
    public Transform targetTransform;
    
    [SerializeField] private float arrowSpeed;
    private float lookTimer;
    private float lookCooldown = 0.1f;
    public bool isPlayer;

    private Vector3 targetPos;

    
    private void Start()
    {
        if(targetTransform == null)
        {
            BoltNetwork.Destroy(gameObject);
            //Lean.Pool.LeanPool.Despawn(gameObject);
        }
    }

    void Update()
    {
        if (targetTransform != null)
        {
            targetPos = targetTransform.position;
        }
        LookEnemy();
        MoveToTarget();
    }

    private void FixedUpdate()
    {
        DistanceCheck();
    }

    private void MoveToTarget()
    {
        var step = arrowSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos+Vector3.up, step);
    }
    
    private void LookEnemy()
    {
        lookTimer += Time.deltaTime;
        
        if (lookTimer > lookCooldown)
        {
            lookTimer = 0;
            transform.LookAt(targetPos+Vector3.up);
        }
    }
    
    private void DistanceCheck()
    {
        
        // If the arrow is close enough to the target deal damage and despawn using objectpool
        if (!(Vector3.Distance(transform.position, targetPos + Vector3.up) < 0.2f)) return;
        
        if(!isPlayer)
        {
            if (targetTransform != null && targetTransform.GetComponent<BoltEntity>().IsAttached)
                targetTransform.GetComponent<Enemy>().TakeDamage(arrowDamage);
        }
        if (isPlayer)
        {
            if (targetTransform!= null && targetTransform.GetComponent<BoltEntity>().IsAttached)
                targetTransform.GetComponent<Player>().TakeDamage(arrowDamage);
        }
        //Lean.Pool.LeanPool.Despawn(gameObject);
        BoltNetwork.Destroy(gameObject);
    }
}
