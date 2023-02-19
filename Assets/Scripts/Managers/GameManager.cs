using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool _isGameOver;
    [SerializeField]private GameObject endPanel,winPanel;
    [SerializeField]private AddressablesManager addressablesManager;
    private int playerKill, enemyKill;
    [SerializeField] private TextMeshProUGUI playerKillText;
    [SerializeField] private TextMeshProUGUI enemyKillText;

    public int PlayerKill
    {
        get => playerKill;

        set
        {
            playerKill = value;
            playerKillText.text = playerKill.ToString(); //Update ui when set;
        }
        
    }

    public int EnemyKill
    {
        get => enemyKill;
        set
        {
            enemyKill = value;
            enemyKillText.text = enemyKill.ToString(); //Update ui when set;
        }
    }

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
            Invoke(nameof(RestartGame),1f);
        }
    }

    public void Win()
    {
        if (_isGameOver) return;
        winPanel.SetActive(true);
        Invoke(nameof(CallAddressable),2f);
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CallAddressable()
    {
        addressablesManager.ShowSceneLoadPopup();
    }
}
