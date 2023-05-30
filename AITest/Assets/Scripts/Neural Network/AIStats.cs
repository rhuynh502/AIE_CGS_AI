using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/AIStats", fileName = "NewPlayerStats", order = 0)]
public class AIStats : ScriptableObject
{
    public PlayerStats playerStats;
    public int amountOfInputs;
    public int amountOfOutputs;
    public int amountOfHiddenNodes;
    public float angleSpread;
    public LayerMask layersToScan;
    public float lifetime;
    public bool canLearn;
    public float timeAdd;
    public float timeRemove;
}
