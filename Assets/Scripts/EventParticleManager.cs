using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventParticleManager : MonoBehaviour
{
    public static event Action<Transform> OnHealParticleSpawnEvent;
    public static event Action<Transform> OnDamageParticleSpawnEvent;
    public static event Action<Transform> OnAuraParticleSpawnEvent;
    public static event Action<Transform> OnDeathParticleSpawnEvent;
    

    [SerializeField] private GameObject auraParticle;
    [SerializeField] private GameObject[] healParticles;
    [SerializeField] private GameObject[] takeDamageParticles;
    [SerializeField] private GameObject deathParticle;

    private Vector3 particleOffset = new Vector3(0, 1f, 0);
    
    public static void OnHealParticleSpawn(Transform pos)
    {
        OnHealParticleSpawnEvent?.Invoke(pos);
    }
    public static void OnDamageParticleSpawn(Transform pos)
    {
        OnDamageParticleSpawnEvent?.Invoke(pos);
    }
    public static void OnAuraParticleSpawn(Transform pos)
    {
        OnAuraParticleSpawnEvent?.Invoke(pos);
    }

    public static void OnDeathParticleSpawn(Transform pos)
    {
        OnDeathParticleSpawnEvent?.Invoke(pos);
    }

    private void OnEnable()
    {
        OnHealParticleSpawnEvent += SpawnHealParticle;
        OnDamageParticleSpawnEvent += SpawnDamageParticle;
        OnAuraParticleSpawnEvent += SpawnHealAura;
        OnDeathParticleSpawnEvent += SpawnDeathParticle;

    }

    private void OnDisable()
    {
        OnHealParticleSpawnEvent -= SpawnHealParticle;
        OnDamageParticleSpawnEvent -= SpawnDamageParticle;
        OnAuraParticleSpawnEvent -= SpawnHealAura;
        OnDeathParticleSpawnEvent -= SpawnDeathParticle;
    }
    
    private void SpawnHealParticle(Transform pos)
    {
        var rnds = Random.Range(0, healParticles.Length);
        Instantiate(healParticles[rnds],pos.position+particleOffset,Quaternion.identity,this.transform);
    }
    
    
    private void SpawnDamageParticle(Transform pos)
    {
        var rnd = Random.Range(0, takeDamageParticles.Length);
        Instantiate(takeDamageParticles[rnd],pos.position+particleOffset,Quaternion.identity,this.transform);
    }

    private void SpawnHealAura(Transform pos)
    {
        var auraClone = Instantiate(auraParticle, pos.position, Quaternion.Euler(90,0,0), pos.transform);
        Destroy(auraClone,2f);
    }

    private void SpawnDeathParticle(Transform pos)
    {
        BoltNetwork.Instantiate(deathParticle, pos.position, Quaternion.identity);
       
    }

   

   
}
