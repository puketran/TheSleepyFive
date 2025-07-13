using System;
using PurrNet;
using UnityEngine;

public abstract class Item : AInteractable
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemPicture;
    [SerializeField] private Rigidbody rb;
    public string ItemName => itemName;
    public Sprite ItemPicture => itemPicture;

    // protected override void OnSpawned()
    // {
    //     base.OnSpawned();

    //     rb.isKinematic = !isServer; // Disable physics on client side
    // }

    public void SetKinematic(bool isKinematic)
    {
        if (rb != null)
        {
            rb.isKinematic = isKinematic;
        }
    }

    protected override void OnOwnerChanged(PlayerID? oldOwner, PlayerID? newOwner, bool asServer)
    {
        base.OnOwnerChanged(oldOwner, newOwner, asServer);

        if (PlayerInventory.localInventory.IsHoldingItem(this))
        {
            rb.isKinematic = true;
            return;
        }
        rb.isKinematic = !isOwner; // Disable physics on client side
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

    public virtual void UseItem()
    {

    }

    public virtual void ConsumeItem()
    {

    }

    public virtual bool CanConsumeItem()
    {
        return false;
    }

    [ObserversRpc(bufferLast: true)]
    public void SetLayer(int ignoreRayCastLayer)
    {
        SetLayerRecursive(gameObject, ignoreRayCastLayer);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}
