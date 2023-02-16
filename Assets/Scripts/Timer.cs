using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float timerValue = 90.0f;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TextMeshProUGUI spawnRateText;
    void Update()
    {
        timerValue -= Time.deltaTime;
        enemySpawner.spawnRate -= 0.015f * Time.deltaTime;
        spawnRateText.text = "Spawn Time:" + enemySpawner.spawnRate.ToString("0.#");
        timerText.text = "Time Left:" + Mathf.Round(timerValue); //.ToString();

        
        if (timerValue < 0)
        {
            GameManager.Instance.Win();
            this.enabled = false;
        }
    }
}
