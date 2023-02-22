using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AddressablesManager : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject towerRef;
    [SerializeField] private Vector3 testPos;
    [SerializeField] private AssetReference assetRefScene;
    
    [SerializeField] TextMeshProUGUI popupText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private GameObject popupPanel;

    private void Awake()
    {
        Caching.ClearCache();
    }

    public void SpawnAddressablePrefab()
    {
        Debug.Log("Spawned Addressables");
        testPos.x = Random.Range(-6, 6);
        Addressables.InstantiateAsync(towerRef, testPos, Quaternion.identity,transform).Completed += Spawn_Complete;
    }
    public void ShowSceneLoadPopup()
    {
        var downloadSizeCall = Addressables.GetDownloadSizeAsync(assetRefScene.RuntimeKey);
        downloadSizeCall.Completed += handle =>
        {
            var sizeInBytes = handle.Result;
            var sizeInMb = sizeInBytes / (1024f * 1024f);
            var popupString = $"This scene size ({sizeInMb:F1} MB) Do you want to continue?";
            popupText.text = popupString;
            popupPanel.SetActive(true);
        };
        
        yesButton.onClick.RemoveAllListeners();     // Remove listener before add input avoid listener bug
        yesButton.onClick.AddListener(() =>     // Yes button listener
        {
            popupPanel.SetActive(false);
            AddressableScene();
        });
        
        noButton.onClick.RemoveAllListeners();      // Remove listener before add input avoid listener bug
        noButton.onClick.AddListener(() =>      // No button listener
        {
            popupPanel.SetActive(false);
            GameManager.Instance.RestartGame();
        });
        
    }

    private void AddressableScene()
    {
        Debug.Log("Scene Loaded");
        Addressables.LoadSceneAsync(assetRefScene);
    }
    private void Scene_Complete(AsyncOperationHandle<SceneInstance> sceneHandle)
    {
        //Load complete do something
    }

    private void Spawn_Complete(AsyncOperationHandle<GameObject> handle)
    {
        #if UNITY_EDITOR
        FixShaders.FixShadersForEditor(handle.Result);
        #endif
    }
}



