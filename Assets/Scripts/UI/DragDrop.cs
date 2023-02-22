using System;
using Photon.Bolt;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class DragDrop : GlobalEventListener
{
    [SerializeField] private GameObject archerPrefab,knighPrefab,eaglePrefab,shamanPrefab;
    [SerializeField] private LayerMask layerMask;
    private GameObject cloneArcher;
    [SerializeField]private CanvasGroup dragCanvGroup;
    private Camera mainCamera;
    private bool selected;
    private GameObject newSpawn;

    [SerializeField] private GameObject archerReal,knightReal,eagleReal,shamanReal;
    [SerializeField] private GameObject enemyArcher, enemyKnight, enemyEagle, enemyShaman;
    
    [SerializeField] private Transform unitHolder;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount < 1) return;

        var touch = Input.GetTouch(0);
        var ray = mainCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out var raycastHit, layerMask) && selected)
        {
                cloneArcher.transform.position = raycastHit.point;
                dragCanvGroup.alpha = 0.1f;
        }
        if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled && selected)
        {
            BoltNetwork.Instantiate(newSpawn, cloneArcher.transform.position, Quaternion.identity);
            Destroy(cloneArcher);
            //var cloneAgent = cloneArcher.GetComponent<NavMeshAgent>();
            //NavMeshHit closestHit;
            //if (NavMesh.SamplePosition (transform.position, out closestHit, 100f, NavMesh.AllAreas)) {
                //transform.position = closestHit.position;
                //cloneAgent.enabled = true;
                //cloneArcher.GetComponent<Player>().enabled = true;
                dragCanvGroup.alpha = 1;
                selected = false;
            //}
        }
    }

    // Spawn units with costs
    public void SpawnArcher() 
    {
        UnitSpawner(archerPrefab,2);
        if (BoltManager.Instance.playerCount == 1)
        {
            newSpawn = archerReal;
        }
        else
        {
            newSpawn = enemyArcher;
        }
        
    }

    public void SpawnKnight()
    {
        UnitSpawner(knighPrefab,3);
        if (BoltManager.Instance.playerCount == 1)
        {
            newSpawn = knightReal;
        }
        else
        {
            newSpawn = enemyKnight;
        }
    }

    public void SpawnEagle()
    {
        UnitSpawner(eaglePrefab,4);
        if (BoltManager.Instance.playerCount == 1)
        {
            newSpawn = eagleReal;
        }
        else
        {
            newSpawn = enemyEagle;
        }
    }

    public void SpawnShaman()
    {
        UnitSpawner(shamanPrefab,3);
        if (BoltManager.Instance.playerCount == 1)
        {
            newSpawn = shamanReal;
        }
        else
        {
            newSpawn = enemyShaman;
        }
    }
    private void UnitSpawner(GameObject go, int value)
    {
        if (!selected && BuyManager.Instance.mana >= value)
        {
             cloneArcher = Instantiate(go, Input.mousePosition, Quaternion.identity);
             BuyManager.Instance.BuySoldier(value);
             selected = true;
        }
    }
}