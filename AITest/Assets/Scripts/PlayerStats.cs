using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerStats", fileName = "NewPlayerStats", order = 0)]
public class PlayerStats : ScriptableObject
{
    public int moveSpeed;
    public float turnSpeed;
    public float angleSpread;

}
