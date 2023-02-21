using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Assets", menuName = "Character/Data", order = 1)]
[System.Serializable]  
public class ScriptableSettings : ScriptableObject
{
     public string unitName;
     public float damage;
     public float moveSpeed;
     public float health;
     public float distance;
     public float healAmount;
     public float attackSpeed;
     public float multiplier;
     public int level;
     
     public bool isMelee;
     public bool isRange;
     public bool isBase;
     public bool isHealer;
}