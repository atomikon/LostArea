using UnityEngine;
using FishNet.Managing;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;

public class SimpleSpawn : MonoBehaviour
{
    [Header("FishNet")]
    public NetworkManager networkManager;

    [Header("Spawn")]
    public NetworkObject playerPrefab;
    public Transform spawnPoint;

    private void Awake()
    {
        if (networkManager == null)
            networkManager = FindFirstObjectByType<NetworkManager>();

        if (spawnPoint == null)
            spawnPoint = transform;
    }

    private void OnEnable()
    {
        if (networkManager != null)
            networkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
    }

    private void OnDisable()
    {
        if (networkManager != null)
            networkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
    }

    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (!networkManager.IsServer)
            return;

        if (args.ConnectionState != RemoteConnectionState.Started)
            return;

        if (playerPrefab == null)
            return;

        Vector3 pos = spawnPoint.position;
        Quaternion rot = spawnPoint.rotation;

        NetworkObject nob = Instantiate(playerPrefab, pos, rot);
        networkManager.ServerManager.Spawn(nob.gameObject, conn);
    }
}