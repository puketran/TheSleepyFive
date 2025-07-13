using System;
using System.Collections.Generic;
using PurrNet;
using PurrNet.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> allItems = new();
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private List<InventorySlot> slots;
    [SerializeField] private List<ActionSlot> actionSlots = new();

    [PurrReadOnly, SerializeField] private InventoryItemData[] _inventoryData;
    private ActionSlot _activeActionSlot;

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
        canvasGroup.blocksRaycasts = toggle;
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
                itemName = item.ItemName,
                itemPicture = item.ItemPicture
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
        if (oldSlotIndex == -1)
        {
            Debug.LogWarning("Invalid slot indices for item move.");
            return;
        }
        var oldData = _inventoryData[oldSlotIndex];
        _inventoryData[oldSlotIndex] = default;
        _inventoryData[newSlotIndex] = oldData;
    }

    public void DropItem(InventoryItem inventoryItem)
    {
        var itemData = Array.Find(_inventoryData, data => data.inventoryItem == inventoryItem);
        if (itemData.inventoryItem == null)
        {
            Debug.LogWarning("Item not found in inventory data.");
            return;
        }

        var itemToSpawn = allItems.Find(item => item.ItemName == itemData.itemName);
        if (itemToSpawn == null)
        {
            Debug.LogWarning($"Item {itemData.itemName} not found in allItems list.");
            return;
        }

        // PlayerInventory.localInventory.UnequipItem(itemToSpawn);

        var posToSpawn = NetworkPlayerController.LocalPlayerController.transform.position + NetworkPlayerController.LocalPlayerController.transform.forward + Vector3.up;
        var item = Instantiate(itemToSpawn, posToSpawn, Quaternion.identity);
        if (DeductItem(inventoryItem) <= 0)
        {
            PlayerInventory.localInventory.UnequipItem(itemToSpawn);
        }
    }

    public void ConsumeItem(Item item)
    {
        var itemData = Array.Find(_inventoryData, data => data.itemName == item.ItemName);
        if (itemData.inventoryItem == null)
        {
            Debug.LogWarning("Item not found in inventory data.");
            return;
        }
        // var item = Instantiate(itemToSpawn, posToSpawn, Quaternion.identity);
        if (DeductItem(itemData) <= 0)
        {
            PlayerInventory.localInventory.UnequipItem(item);
        }
    }

    private int DeductItem(InventoryItemData inventoryItemData)
    {
        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (data.inventoryItem != inventoryItemData.inventoryItem)
                continue;

            data.amount--;
            if (data.amount <= 0)
            {
                // Remove item from inventory
                _inventoryData[i] = default;
                slots[i].SetItem(null);
                Destroy(data.inventoryItem.gameObject);
                return 0;
            }
            else
            {
                // Update item amount
                data.inventoryItem.Init(data.itemName, data.itemPicture, data.amount);
                _inventoryData[i] = data;
                return data.amount;
            }
        }
        return 0;
    }

    private int DeductItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (data.inventoryItem != inventoryItem)
                continue;

            data.amount--;
            if (data.amount <= 0)
            {
                // Remove item from inventory
                _inventoryData[i] = default;
                slots[i].SetItem(null);
                Destroy(data.inventoryItem.gameObject);
                return 0;
            }
            else
            {
                // Update item amount
                data.inventoryItem.Init(data.itemName, data.itemPicture, data.amount);
                _inventoryData[i] = data;
                return data.amount;
            }
        }
        return 0;
    }

    private Item GetItemByName(string itemName)
    {
        var item = allItems.Find(i => i.ItemName == itemName);
        return item;
    }

    private Item GetItemByActionSlot(ActionSlot actionSlot)
    {
        var inventorySlot = actionSlot.GetComponent<InventorySlot>();
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i] == inventorySlot)
                return GetItemByName(_inventoryData[i].itemName);
        }
        return null;
    }

    public void SetActionSlotActive(ActionSlot actionSlot)
    {
        if (_activeActionSlot == actionSlot)
            return;
        if (_activeActionSlot != null)
        {
            PlayerInventory.localInventory.UnequipItem(GetItemByActionSlot(_activeActionSlot));
            _activeActionSlot.ToggleActive(false);
        }
        actionSlot.ToggleActive(true);
        _activeActionSlot = actionSlot;

        PlayerInventory.localInventory.EquipItem(GetItemByActionSlot(actionSlot));
    }

    [Serializable]
    public struct InventoryItemData
    {
        public string itemName;
        public int amount;
        public InventoryItem inventoryItem;
        public Sprite itemPicture;

    }
}
