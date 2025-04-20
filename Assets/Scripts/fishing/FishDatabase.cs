using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishDatabase", menuName = "FishDatabase")]
public class FishDatabase : ScriptableObject
{
    public List<FishData> allFish;

    private Dictionary<string, FishData> lookup;

    public FishData GetFishByName(string name)
    {
        if (lookup == null)
        {
            lookup = new Dictionary<string, FishData>();
            foreach (var fish in allFish)
            {
                if (!lookup.ContainsKey(fish.fishName))
                    lookup.Add(fish.fishName, fish);
            }
        }

        lookup.TryGetValue(name, out var result);
        return result;
    }
}
