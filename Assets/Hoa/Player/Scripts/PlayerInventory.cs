using System;
using PurrNet;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public static PlayerInventory localInventory;
    [SerializeField] private Transform _itemPoint;
    [SerializeField] private KeyCode useItemKey, consumeItemKey;
    private Item _itemInHand;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isOwner)
            return;

        localInventory = this;
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();

        if (!isOwner)
            return;

        localInventory = null;
    }

    public void EquipItem(Item item)
    {
        if (!item)
        {
            return;
        }

        Debug.Log($"Equipping item: {item.ItemName}");
        _itemInHand = Instantiate(item, _itemPoint.position, _itemPoint.rotation, _itemPoint);
        _itemInHand.SetKinematic(true);
        // Debug.Log($"Equipped item: {newItem.ItemName} at position: {_itemPoint.position}");
    }

    public void UnequipItem(Item item)
    {
        if (!item)
        {
            return;
        }

        if (!_itemInHand)
            return;

        if (_itemInHand.ItemName != item.ItemName)
        {
            Debug.LogWarning($"Trying to unequip item: {item.ItemName}, but currently holding: {_itemInHand.ItemName}");
            return;
        }

        Destroy(_itemInHand.gameObject);
        _itemInHand = null;

        Debug.Log($"UnEquipping item: {item.ItemName}");
    }

    public bool IsHoldingItem(Item item)
    {
        if (!_itemInHand)
            return false;
        return item == _itemInHand;
    }

    private void Update()
    {
        if (Input.GetKeyDown(useItemKey) || Input.GetMouseButtonDown(0))
        {
            UseItem();
        }
        if (Input.GetKeyDown(consumeItemKey))
        {
            ConsumeItem();
        }
    }

    void UseItem()
    {
        if (!_itemInHand)
        {
            Debug.LogWarning("No item in hand to use.");
            return;
        }
        if (_itemInHand is Tool tool)
        {
            tool.UseItem();
        }
        else
        {
            Debug.LogWarning($"Cannot use item: {_itemInHand.ItemName}. Only tools can be used.");
        }
    }

    void ConsumeItem()
    {
        if (!_itemInHand)
        {
            Debug.LogWarning("No item in hand to consume.");
            return;
        }
        if (_itemInHand is Consumable consumable)
        {
            consumable.ConsumeItem();
        }
        else
        {
            Debug.LogWarning($"Cannot consume item: {_itemInHand.ItemName}. Only consumables can be consumed.");
        }
    }
}
