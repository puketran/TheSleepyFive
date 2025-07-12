using PurrNet;
using UnityEngine;

public class Item : AInteractable
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemPicture;
    [SerializeField] private Rigidbody rb;
    public string ItemName => itemName;
    public Sprite ItemPicture => itemPicture;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        rb.isKinematic = !isServer; // Disable physics on client side
    }

    public override void Interact()
    {
        Pickup();
    }

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

    public override void OnHover()
    {
        base.OnHover();
        Debug.Log("Hovering over item: " + itemName);
    }

    public override void OnStopHover()
    {
        base.OnStopHover();
        Debug.Log("Stopped hovering over item: " + itemName);
    }
}
