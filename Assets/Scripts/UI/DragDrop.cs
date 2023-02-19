using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour
{
    [SerializeField] private GameObject archerPrefab,knighPrefab,eaglePrefab,shamanPrefab;
    [SerializeField] private LayerMask layerMask;
    private GameObject cloneArcher;
    [SerializeField]private CanvasGroup dragCanvGroup;
    private Camera mainCamera;
    private bool selected;

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
                dragCanvGroup.alpha = 0.5f;
        }

        if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled && selected)
        {
            cloneArcher.GetComponent<Player>().enabled = true;
            cloneArcher.GetComponent<NavMeshAgent>().enabled = true;
            dragCanvGroup.alpha = 1;
            selected = false;
        }
    }

    public void SpawnArcher()
    {
        UnitSpawner(archerPrefab,2);
    }

    public void SpawnKnight()
    {
        UnitSpawner(knighPrefab,3);
    }

    public void SpawnEagle()
    {
        UnitSpawner(eaglePrefab,4);
    }

    public void SpawnShaman()
    {
        UnitSpawner(shamanPrefab,3);
    }
    private void UnitSpawner(GameObject go, int value)
    {
        if (!selected && BuyManager.Instance.mana >= value)
        {
             cloneArcher = Instantiate(go, Input.mousePosition, Quaternion.identity, unitHolder);
             BuyManager.Instance.BuySoldier(value);
            selected = true;
        }
    }
}