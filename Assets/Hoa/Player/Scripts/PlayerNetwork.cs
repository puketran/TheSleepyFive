using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using StarterAssets;

[Serializable]
public struct PlayerData : INetworkSerializable
{
    public bool isActive;
    public int score;
    public string playerName;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref isActive);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref playerName);
    }
}

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform spawnedObjectPrefab;
    [SerializeField] private Transform rootCamera;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    public Transform RootCamera
    {
        get => rootCamera;
    }

    private NetworkVariable<PlayerData> networkPlayerData = new NetworkVariable<PlayerData>(
        new PlayerData { isActive = true, score = 0, playerName = "Player" },
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to NetworkVariable changes
        networkPlayerData.OnValueChanged += OnPlayerDataChanged;

        // Initialize player data if owner
        if (IsOwner)
        {
            var newData = new PlayerData
            {
                isActive = true,
                score = 0,
                playerName = $"Player_{OwnerClientId}"
            };
            networkPlayerData.Value = newData;

            playerInput.enabled = true; // Enable player input for the owner
            thirdPersonController.enabled = true; // Enable third person controller for the owner
            starterAssetsInputs.enabled = true; // Enable StarterAssetsInputs for the owner
        }
    }

    private void OnPlayerDataChanged(PlayerData previousValue, PlayerData newValue)
    {
        Debug.Log($"Player data changed: {newValue.playerName}, Active: {newValue.isActive}, Score: {newValue.score}");
    }

    public void UpdateScore(int newScore)
    {
        if (!IsOwner) return;

        var currentData = networkPlayerData.Value;
        currentData.score = newScore;
        networkPlayerData.Value = currentData;
    }

    public void SetPlayerActive(bool active)
    {
        if (!IsOwner) return;

        var currentData = networkPlayerData.Value;
        currentData.isActive = active;
        networkPlayerData.Value = currentData;
    }

    // Update is called once per frame
    void Update()
    {
        // if (!IsOwner) return;

        // HandleMovement();
        // HandleInput();
    }

    private void HandleInput()
    {
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     var currentData = networkPlayerData.Value;
        //     UpdateScore(currentData.score + 1);
        // }

        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     SendMessageToServerRpc("Hello from client!");
        // }

        // if (Input.GetKeyDown(KeyCode.N))
        // {
        //     SpawnObjectServerRpc();
        // }
    }

    [ServerRpc]
    private void SendMessageToServerRpc(string message)
    {
        Debug.Log($"Server received message from client {OwnerClientId}: {message}");

        // Server can process the message and potentially respond to all clients
        ProcessServerMessage(message);
    }

    [ServerRpc]
    private void SpawnObjectServerRpc()
    {
        if (spawnedObjectPrefab == null)
        {
            Debug.LogWarning("Spawned object prefab is not assigned!");
            return;
        }

        // Spawn the object at the player's position with a slight offset
        Vector3 spawnPosition = transform.position + Vector3.forward * 2f;
        GameObject spawnedObject = Instantiate(spawnedObjectPrefab.gameObject, spawnPosition, Quaternion.identity);

        // Get the NetworkObject component and spawn it on the network
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
            Debug.Log($"Player {OwnerClientId} spawned a network object at {spawnPosition}");
        }
        else
        {
            Debug.LogError("Spawned prefab doesn't have a NetworkObject component!");
            Destroy(spawnedObject);
        }
    }

    private void ProcessServerMessage(string message)
    {
        // Server-side logic for processing the message
        Debug.Log($"Server processing message: {message}");

        // Example: You could broadcast to all clients, update game state, etc.
        // BroadcastMessageToAllClientsRpc($"Server processed: {message}");
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D keys
        float verticalInput = Input.GetAxis("Vertical");     // W/S keys

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}
