using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyManager : MonoBehaviour
{
    public static BuyManager Instance;
    [SerializeField] private Slider manaSlider;
    public float mana = 10;
    private readonly int maxMana = 10;

    private float regenerationRate = 0.03f;
    private float regenTimer;
    [SerializeField] private TextMeshProUGUI manaText;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        manaSlider.maxValue = maxMana;
        manaSlider.value = mana;
    }

    private void Update()
    {
        HealthRegeneration();
    }


    private void HealthRegeneration()
    {
        regenTimer += Time.deltaTime;
       

        if (regenTimer >= regenerationRate && mana < maxMana)
        {
            regenTimer = 0;
            mana += 0.04f;

            if (mana > maxMana)
                mana = maxMana;
            
            manaSlider.value = mana;
            manaText.text = mana.ToString("0.#");
        }
    }
    
    public void BuySoldier(float value)
    {
        mana -= value;
        manaSlider.value = mana;
    }
}