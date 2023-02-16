using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private GameObject archerPrefab;
    private int xBound = 5;
    
    public void SpawnUnit()
    {
        Vector3 tempPos = transform.position;
        tempPos.x += Random.Range(-xBound, xBound);
        tempPos.y += 0.5f;
        Instantiate(archerPrefab, tempPos, Quaternion.identity,this.transform);
    }
}
