using PurrNet;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemPicture;
    public string ItemName => itemName;
    public Sprite ItemPicture => itemPicture;

    [ContextMenu("Pickup Item")]
    public void Pickup()
    {
        if (!InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            Debug.LogError("InventoryManager instance not found!");
            return;
        }
        inventoryManager.AddItem(this);
        Destroy(gameObject);
    }
}
