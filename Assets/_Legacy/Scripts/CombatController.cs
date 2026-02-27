using FishNet.Object;
using UnityEngine;

public class CombatController : NetworkBehaviour
{
    [Header("Refs")]
    public Transform view;
    public Health selfHealth;

    [Header("Tuning")]
    public float range = 2.2f;
    public float lightDamage = 12f;
    public float heavyDamage = 25f;
    public LayerMask hitMask = ~0;

    private void Awake()
    {
        if (selfHealth == null) selfHealth = GetComponent<Health>();
        if (view == null) view = Camera.main != null ? Camera.main.transform : transform;
    }

    public void RequestLightAttack()
    {
        if (!IsOwner) return;
        LightAttackServerRpc();
    }

    public void RequestHeavyAttack()
    {
        if (!IsOwner) return;
        HeavyAttackServerRpc();
    }

    [ServerRpc]
    private void LightAttackServerRpc()
    {
        ServerDoHit(lightDamage);
    }

    [ServerRpc]
    private void HeavyAttackServerRpc()
    {
        ServerDoHit(heavyDamage);
    }

    [Server]
    private void ServerDoHit(float damage)
    {
        if (selfHealth != null && selfHealth.IsDead) return;

        Vector3 origin = (view != null) ? view.position : transform.position + Vector3.up * 1.5f;
        Vector3 dir = (view != null) ? view.forward : transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            Health h = hit.collider.GetComponentInParent<Health>();
            if (h != null && h != selfHealth)
                h.ServerDamage(damage);
        }
    }
}
