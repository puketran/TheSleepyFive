using System;
using System.Collections.Generic;
using PurrNet;
using PurrNet.Utils;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private List<InventorySlot> slots;

    [PurrReadOnly, SerializeField] private InventoryItemData[] _inventoryData;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
        _inventoryData = new InventoryItemData[slots.Count];
        ToggleInventory(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = canvasGroup.alpha > 0;
            ToggleInventory(!isOpen);
        }
    }

    private void ToggleInventory(bool toggle)
    {
        canvasGroup.blocksRaycasts = !toggle;
        canvasGroup.alpha = toggle ? 1 : 0;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = toggle;
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<InventoryManager>();
    }

    private bool TryStackItem(Item item)
    {

        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (string.IsNullOrEmpty(data.itemName))
            {
                // Empty slot, we can add a new item here
                continue;
            }

            if (data.itemName != item.ItemName)
            {
                continue;
            }

            data.amount++;
            data.inventoryItem.Init(item.ItemName, item.ItemPicture, data.amount);
            _inventoryData[i] = data;
            return true;
        }

        return false;
    }

    public void AddItem(Item item)
    {
        if (!TryStackItem(item))
        {
            AddNewItem(item);
        }

    }

    private void AddNewItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty)
                continue;


            var inventoryItem = Instantiate(itemPrefab, slot.transform);
            inventoryItem.Init(item.ItemName, item.ItemPicture, 1);
            var itemData = new InventoryItemData()
            {
                amount = 1,
                inventoryItem = inventoryItem,
                itemName = item.ItemName
            };
            _inventoryData[i] = itemData;
            slot.SetItem(inventoryItem);
            break;
        }
    }

    public void ItemMoved(InventoryItem item, InventorySlot newSlot)
    {
        var newSlotIndex = slots.IndexOf(newSlot);
        var oldSlotIndex = Array.FindIndex(_inventoryData, data => data.inventoryItem == item);
        if (newSlotIndex < 0 || oldSlotIndex < 0 || newSlotIndex == oldSlotIndex)
        {
            Debug.LogWarning("Invalid slot indices for item move.");
            return;
        }
        var oldData = _inventoryData[oldSlotIndex];
        _inventoryData[oldSlotIndex] = default;
        _inventoryData[newSlotIndex] = oldData;


    }

    [Serializable]
    public struct InventoryItemData
    {
        public string itemName;
        public int amount;
        public InventoryItem inventoryItem;

    }
}
