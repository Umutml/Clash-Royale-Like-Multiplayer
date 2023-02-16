using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour
{
    [SerializeField] private GameObject archerPrefab,knighPrefab,eaglePrefab;
    [SerializeField] private LayerMask layerMask;

    private GameObject cloneArcher;

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
        }

        if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled && selected)
        {
            cloneArcher.GetComponent<Player>().enabled = true;
            cloneArcher.GetComponent<NavMeshAgent>().enabled = true;
            selected = false;
        }

    }


    public void SpawnArcher()
    {
        if (!selected && BuyManager.Instance.mana >= 2)
        {
            cloneArcher = Instantiate(archerPrefab, Input.mousePosition, Quaternion.identity, unitHolder);
            BuyManager.Instance.BuySoldier(2);
            selected = true;
        }
    }

    public void SpawnKnight()
    {
        if (!selected && BuyManager.Instance.mana >= 3)
        {
            cloneArcher = Instantiate(knighPrefab, Input.mousePosition, Quaternion.identity, unitHolder);
            BuyManager.Instance.BuySoldier(3);
            selected = true;
        }
    }

    public void SpawnEagle()
    {
        if (!selected && BuyManager.Instance.mana >= 3)
        {
            cloneArcher = Instantiate(eaglePrefab, Input.mousePosition, Quaternion.identity, unitHolder);
            BuyManager.Instance.BuySoldier(3);
            selected = true;
        }
    }
}