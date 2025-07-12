using System.Collections.Generic;
using Cinemachine;
using PurrNet;
using StarterAssets;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _playerCamera;
    [SerializeField]
    private ThirdPersonController _thirdPersonController;
    [SerializeField]
    private List<MonoBehaviour> _toEnableBehaviors;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isOwner)
        {
            Destroy(_playerCamera.gameObject);
            EnableBehaviors(false);
            return;
        }


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EnableBehaviors(true);

        _thirdPersonController.OnInit();
    }

    private void EnableBehaviors(bool enable)
    {
        foreach (var behavior in _toEnableBehaviors)
        {
            behavior.enabled = enable;
        }
    }
    protected override void OnDespawned()
    {
        base.OnDespawned();

        if (!isOwner)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
