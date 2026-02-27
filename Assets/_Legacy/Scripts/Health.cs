using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [Header("Config")]
    public float maxHp = 100f;

    public readonly SyncVar<float> hp = new();

    public bool IsDead => hp.Value <= 0f;

    private void Awake()
    {
        if (hp.Value <= 0f) hp.Value = maxHp;
    }

    [Server]
    public void ServerReset()
    {
        hp.Value = maxHp;
    }

    [Server]
    public void ServerDamage(float amount)
    {
        if (amount <= 0f) return;
        if (IsDead) return;
        hp.Value = Mathf.Max(0f, hp.Value - amount);
    }

    [Server]
    public void ServerHeal(float amount)
    {
        if (amount <= 0f) return;
        if (IsDead) return;
        hp.Value = Mathf.Min(maxHp, hp.Value + amount);
    }
}
