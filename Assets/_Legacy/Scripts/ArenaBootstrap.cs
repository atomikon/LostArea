using UnityEngine;
using FishNet.Managing;

public class ArenaBootstrap : MonoBehaviour
{
    public NetworkManager networkManager;

    private void Awake()
    {
        if (networkManager == null)
            networkManager = FindFirstObjectByType<NetworkManager>();
    }

    private void Start()
    {
        if (networkManager == null)
        {
            Debug.LogError("ArenaBootstrap: NetworkManager not found in scene.");
            return;
        }

        // Auto-start Host for quick arena testing.
        if (!networkManager.IsServer && !networkManager.IsClient)
            networkManager.ServerManager.StartConnection();

        if (!networkManager.IsClient)
            networkManager.ClientManager.StartConnection();
    }
}