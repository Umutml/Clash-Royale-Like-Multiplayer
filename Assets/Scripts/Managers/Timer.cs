using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using TMPro;
using UnityEngine;

public class Timer : GlobalEventListener
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float timerValue = 90.0f;
    //[SerializeField] private EnemySpawner enemySpawner;
    //[SerializeField] private TextMeshProUGUI spawnRateText;
    
    
    private bool canStart;
    
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private int timerCounts;

    void Update()
    {
        if (BoltNetwork.IsServer)
        {
            hostPanel.SetActive(true);
        }
        
        // foreach (var session in BoltNetwork.SessionList)
        // {
        //     playercount = session.Value.ConnectionsCurrent;
        // }
        
        
        //foreach (var session in BoltNetwork.SessionList)
        //{
        //    timerCounts = session.Value.ConnectionsCurrent;

        //}
        
        //if (timerCounts >= 1)
        //{
        //    canStart = true;
        //}


        if(GameManager.Instance.pCount  >= 1)
        {
            canStart = true;
        }

        if (!canStart)
        {
            return;
        }
        
        timerValue -= Time.deltaTime;
        //enemySpawner.spawnRate -= 0.015f * Time.deltaTime;
        //spawnRateText.text = "Spawn Time:" + enemySpawner.spawnRate.ToString("0.#");
        timerText.text = Mathf.Round(timerValue).ToString();

        if (timerValue < 0)
        {
            GameManager.Instance.Win();
            this.enabled = false;
        }
    }
    
    //public override void OnEvent(TimerStart count)
    //{
      //  playercount = count.playerCounts;
   // }
   
  
}
