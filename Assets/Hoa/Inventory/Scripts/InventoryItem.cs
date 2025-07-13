using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private TMP_Text amountText;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;

    private Image _itemImage;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _itemImage = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Dragging: " + gameObject.name);
        _originalParent = _rectTransform.parent;

        _canvasGroup.blocksRaycasts = false;
        _rectTransform.SetParent(_rectTransform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Dropped outside of a valid slot, returning to original position: ");
        if (eventData.pointerEnter == null || eventData.pointerEnter.TryGetComponent(out InventorySlot slot) == false)
        {

            _rectTransform.SetParent(_originalParent);
            SetAvailable();
        }
    }

    public void SetAvailable()
    {
        _canvasGroup.blocksRaycasts = true;
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Init(string itemName, Sprite itemPicture, int amount)
    {
        _itemImage.sprite = itemPicture;
        amountText.text = amount.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (!InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            Debug.LogError("InventoryManager instance not found!");
            return;
        }

        inventoryManager.DropItem(this);
    }


}
