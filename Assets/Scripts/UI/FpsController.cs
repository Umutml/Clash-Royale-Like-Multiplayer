using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsController : MonoBehaviour
{
    [SerializeField] private int fpsLimit;
    [SerializeField] private float timer, refresh, avgFrameRate;
    [SerializeField] private string display = "{0} FPS";
    [SerializeField] private TextMeshProUGUI fpsText;
    
    
    void Awake()
    {
        Application.targetFrameRate = fpsLimit;
    }

    // Update is called once per frame
    void Update()
    {
        float timeLapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timeLapse;

        if (timer <= 0)
        {
            avgFrameRate = (int)(1f / timeLapse);
            fpsText.text = String.Format(display, avgFrameRate);
        }
    }
}
