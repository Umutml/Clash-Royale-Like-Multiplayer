using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool _isGameOver;
    [SerializeField]private GameObject endPanel,winPanel;
    [SerializeField]private AddressablesManager _addressablesManager;
    
    
    void Start()
    {
        Instance = this;
    }

    public void GameOver()
    {
        if (!_isGameOver)
        {
            _isGameOver = true;
            endPanel.SetActive(true);
            Invoke(nameof(RestartGame),2f);
        }
    }

    public void Win()
    {
        winPanel.SetActive(true);
        Invoke(nameof(RestartGame),2f);
    }

    private void RestartGame()
    {
        _addressablesManager.AddressableScene();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
