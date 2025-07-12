using UnityEngine;
using StarterAssets;
using Unity.Netcode;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    UICanvasControllerInput uiCanvasControllerInput;

    // public override void OnNetworkSpawn()
    // {
    //     if (IsServer)
    //     {
    //         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    //     }

    //     // Find and setup camera for local player
    //     SetupCameraForLocalPlayer();
    // }

    private void OnClientConnected(ulong clientId)
    {
        SetupCameraForLocalPlayer();
    }

    private void SetupCameraForLocalPlayer()
    {
        // Wait a frame to ensure all network objects are spawned
        StartCoroutine(SetupCameraCoroutine());
    }

    private System.Collections.IEnumerator SetupCameraCoroutine()
    {
        yield return new WaitForEndOfFrame();

        // Find the local player's NetworkObject
        NetworkObject localPlayerObject = NetworkManager.Singleton.LocalClient?.PlayerObject;

        if (localPlayerObject != null)
        {
            PlayerNetwork playerNetwork = localPlayerObject.GetComponent<PlayerNetwork>();
            if (playerNetwork != null)
            {
                // Get the RootCamera from PlayerNetwork (assuming it has one)
                Transform rootCamera = playerNetwork.RootCamera;
                if (rootCamera != null)
                {
                    virtualCamera.Follow = rootCamera;
                    // virtualCamera.LookAt = rootCamera;
                    Debug.Log("Virtual camera now follows local player's RootCamera");
                }
                else
                {
                    // Fallback: use the player's transform if no RootCamera found
                    virtualCamera.Follow = playerNetwork.transform;
                    // virtualCamera.LookAt = playerNetwork.transform;
                    Debug.Log("Virtual camera now follows local player (no RootCamera found)");
                }
            }

            StarterAssetsInputs starterAssetsInputs = localPlayerObject.GetComponent<StarterAssetsInputs>();
            if (starterAssetsInputs != null)
            {
                uiCanvasControllerInput.gameObject.SetActive(true);
                uiCanvasControllerInput.starterAssetsInputs = starterAssetsInputs;
            }
        }
    }

    // public override void OnNetworkDespawn()
    // {
    //     if (IsServer)
    //     {
    //         NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    //     }
    // }
}
