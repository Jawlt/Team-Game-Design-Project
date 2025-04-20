using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "FishData", order = 1)]
public class FishData : ScriptableObject
{
    [SerializeField]
    public string fishName;
    public GameObject inventoryItem;
    public int probability;
    public int fishDifficulty; //level 1, 2, 3
    public int fishCost;
}
