using UnityEngine;

[CreateAssetMenu(menuName = "Merchant/Item")]
public class MerchantItem : ScriptableObject
{
    public string itemName;
    public int cost;
    public Sprite icon;

    public enum ItemType { Boat, IslandUnlock }
    public ItemType type;
}
