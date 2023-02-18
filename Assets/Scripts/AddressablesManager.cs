using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AddressablesManager : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject testGameobject;
    [SerializeField] private Vector3 testPos;
    [SerializeField] private AssetReference assetRefScene;
    [SerializeField] private Material castleMat;
    
    
    
    public void SpawnAddressablePrefab()
    {
        Debug.Log("Spawned Addressables");
        testPos.x = Random.Range(-6, 6);
        Addressables.InstantiateAsync(testGameobject, testPos, Quaternion.identity,transform).Completed += Spawn_Complete;
    }

    public void AddressableScene()
    {
        Debug.Log("Scene Loaded");
        Addressables.LoadSceneAsync(assetRefScene).Completed += Scene_Complete;
        
    }

    private void Spawn_Complete(AsyncOperationHandle<GameObject> handle)
    {

#if UNITY_EDITOR
        FixShaders.FixShadersForEditor(handle.Result);
        //FixShaders.FixShadersForEditor(handle.Result.transform.GetChild(0).gameObject);
        // var gend = handle.Result.transform.GetChild(0).GetComponent<MeshRenderer>();
        // gend.material.shader = Shader.Find("Standard");
        //var rend = handle.Result.GetComponent<MeshRenderer>().material;
        //rend.shader = Shader.Find("Standard"); 
#endif
        
    }

    private void Scene_Complete(AsyncOperationHandle<SceneInstance> scene)
    {

    }
}



