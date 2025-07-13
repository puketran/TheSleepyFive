using PurrNet;
using UnityEngine;
using UnityEngine.UI;

public class ActionSlot : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private KeyCode actionKey = KeyCode.Alpha1;
    [SerializeField] private Color activeColor = Color.green;

    private Color _originalColor;

    void Awake()
    {
        _originalColor = slotImage.color;
        ToggleActive(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(actionKey))
            return;

        InstanceHandler.GetInstance<InventoryManager>().SetActionSlotActive(this);
    }

    public void ToggleActive(bool isActive)
    {
        slotImage.color = isActive ? activeColor : _originalColor;
    }
}
