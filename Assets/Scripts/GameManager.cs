using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool _isGameOver;
    [SerializeField]private GameObject endPanel,winPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Invoke(nameof(RestartGame),3f);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
