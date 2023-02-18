using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using AsyncOperation = System.ComponentModel.AsyncOperation;

public class AddressablesManager : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject testGameobject;
    [SerializeField] private Vector3 testPos;
    [SerializeField] private AssetReference assetRefScene;
    [SerializeField] private Texture refTexture;
    
    
    
    public void SpawnAddressablePrefab()
    {
        Debug.Log("Spawned Addressables");
        testPos.x = Random.Range(-6, 6);
        Addressables.InstantiateAsync(testGameobject, testPos, Quaternion.identity,transform).Completed += Spawn_Complete;
    }

    public void AddressableScene()
    {
        Debug.Log("Scene Loaded");
        Addressables.LoadSceneAsync(assetRefScene); //.Completed += Scene_Complete;
    }

    private void Spawn_Complete(AsyncOperationHandle<GameObject> handle)
    {
        var gend = handle.Result.transform.GetChild(0).GetComponent<MeshRenderer>();
        gend.material.shader = Shader.Find("Standard");
        var rend = handle.Result.GetComponent<MeshRenderer>().material;
        rend.shader = Shader.Find("Standard"); 
    }

    /*private void Scene_Complete(AsyncOperationHandle<SceneInstance> scene)
    {
        var renderers = scene.Result.
        
    }*/
    
    
}



