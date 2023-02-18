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
        if (_isGameOver) return;
        
        winPanel.SetActive(true);
        Invoke(nameof(GoNextLevel),2f);
    }

    private void GoNextLevel()
    {
        _addressablesManager.AddressableScene();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
