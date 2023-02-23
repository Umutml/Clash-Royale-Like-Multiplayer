using Photon.Bolt;
using UnityEngine;

public class DragDrop : GlobalEventListener
{
    [SerializeField] private GameObject archerPrefab, knighPrefab, eaglePrefab, shamanPrefab;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private CanvasGroup dragCanvGroup;

    [SerializeField] private GameObject archerReal, knightReal, eagleReal, shamanReal;
    [SerializeField] private GameObject enemyArcher, enemyKnight, enemyEagle, enemyShaman;

    [SerializeField] private Transform unitHolder;
    public int playerCount;


   
    private GameObject cloneArcher;
    private Camera mainCamera;
    private GameObject newSpawn;
    private bool selected;


    private void Start()
    {
        mainCamera = Camera.main;
       
    }

    private void Update()
    {
        if (Input.touchCount < 1) return;

        var touch = Input.GetTouch(0);
        var ray = mainCamera.ScreenPointToRay(touch.position);
       // if (playerCount > 1)
       /// {
       //     layerMask = enemyLayer;
        //}
        if (Physics.Raycast(ray, out var raycastHit, layerMask) && selected)
        {
            cloneArcher.transform.position = raycastHit.point;
            dragCanvGroup.alpha = 0.1f;
        }
        else
        {
            Destroy(cloneArcher);
            selected = false;
            dragCanvGroup.alpha = 1;
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
        UnitSpawner(archerPrefab, 2);

        if (playerCount == 1)
            newSpawn = enemyArcher;
        else
            newSpawn = archerReal;
    }

    public void SpawnKnight()
    {
        UnitSpawner(knighPrefab, 3);
        if (playerCount == 1)
            newSpawn = enemyKnight;
        else
            newSpawn = knightReal;
    }

    public void SpawnEagle()
    {
        UnitSpawner(eaglePrefab, 4);
        if (playerCount == 1)
            newSpawn = enemyEagle;
        else
            newSpawn = eagleReal;
    }

    public void SpawnShaman()
    {
        UnitSpawner(shamanPrefab, 3);
        if (playerCount == 1)
            newSpawn = enemyShaman;
        else
            newSpawn = shamanReal;
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


    public override void OnEvent(TimerStart count)
    {
        playerCount = count.playerCounts;
    }
}