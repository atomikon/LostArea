using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class StaminaComponent : NetworkBehaviour
{
    public float max = 100f;
    public float regenPerSecond = 18f;

    public readonly SyncVar<float> value = new();

    private void Awake()
    {
        if (value.Value <= 0f) value.Value = max;
    }

    [Server]
    public void ServerTick(bool allowRegen)
    {
        if (!allowRegen) return;
        if (regenPerSecond <= 0f) return;
        value.Value = Mathf.Min(max, value.Value + regenPerSecond * Time.deltaTime);
    }

    [Server]
    public bool ServerTrySpend(float cost)
    {
        if (cost <= 0f) return true;
        if (value.Value < cost) return false;
        value.Value -= cost;
        return true;
    }
}
