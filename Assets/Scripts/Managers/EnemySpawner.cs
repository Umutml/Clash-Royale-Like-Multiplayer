using DG.Tweening;
using UnityEngine;
using Photon.Bolt;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnRate = 1.0f;
    private float _spawnTimer;
    [SerializeField] private float xBound = 7.0f;
    [SerializeField] private float zBound= 5.0f;
    [SerializeField] private Transform enemyParent;
    

    void Update()
    {
        _spawnTimer += Time.deltaTime;
        
        if (_spawnTimer >= spawnRate)
        {
            _spawnTimer = 0.0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        int randEnemy = Random.Range(0, enemyPrefabs.Length);
        //var cloneEnemy = BoltNetwork.Instantiate(enemyPrefabs[randEnemy], GetRandomSpawnPosition(), Quaternion.Euler(0,-180,0));
        BoltNetwork.Instantiate(enemyPrefabs[randEnemy], GetRandomSpawnPosition(), Quaternion.Euler(0,-180,0));
        //cloneEnemy.transform.DOMoveY(5, 0.5f).From().SetEase(Ease.Linear);
    }

    
    Vector3 GetRandomSpawnPosition()         // Generates a random spawn position within the spawn area
    {
        Vector3 tempPosition = transform.position;

        tempPosition.x += Random.Range(-xBound, xBound);
        tempPosition.z += Random.Range(-zBound, zBound);
        tempPosition.y += 0.5f;

        return tempPosition;
    }
 
}
