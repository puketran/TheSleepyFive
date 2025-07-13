using System.Linq;
using PurrNet;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactDistance = 4f;

    private Camera _cam;
    private AInteractable[] _currentHoveredInteractables;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        HandleHovers();
        if (!Input.GetKeyDown(KeyCode.P) && !Input.GetMouseButtonDown(1))
        {
            return; // Only check for interactables when the interaction key is pressed
        }
        if (!Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, interactDistance, interactableLayer))
            return;
        var interactables = hit.collider.GetComponents<AInteractable>().ToList();
        // interactables.AddRange(hit.collider.GetComponentsInParent<AInteractable>());
        foreach (var interactable in interactables)
        {
            if (interactable.CanInteract())
            {
                // _currentInteractable = interactable;
                interactable.Interact();
            }
        }

    }

    private void ClearHovers()
    {
        if (_currentHoveredInteractables == null || _currentHoveredInteractables.Length == 0)
            return;
        foreach (var interactable in _currentHoveredInteractables)
        {
            if (interactable)
                interactable.OnStopHover();
        }
        _currentHoveredInteractables = null;
    }

    private void HandleHovers()
    {
        if (!Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, interactDistance, interactableLayer))
        {
            ClearHovers();
            return;
        }

        var interactables = hit.collider.GetComponents<AInteractable>();
        if (interactables == null || interactables.Length == 0)
        {
            ClearHovers();
            return;
        }

        if (_currentHoveredInteractables != null && _currentHoveredInteractables.Length > 0)
        {
            if (!_currentHoveredInteractables[0])
            {
                ClearHovers();
            }
            if (hit.collider.gameObject == _currentHoveredInteractables[0].gameObject)
            {
                // If the first hovered interactable is still the same, no need to update
                return;
            }
        }

        _currentHoveredInteractables = interactables;
        foreach (var interactable in interactables)
        {
            if (interactable.CanInteract())
            {
                interactable.OnHover();
            }
        }
    }
}

public abstract class AInteractable : NetworkBehaviour
{
    public abstract void Interact();

    public virtual void OnHover() { }
    public virtual void OnStopHover() { }

    public virtual bool CanInteract()
    {
        return true;
    }
}
