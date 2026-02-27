using FishNet.Object;
using UnityEngine;

/// <summary>
/// Small helper: after releasing grapple you can keep a short boost.
/// GrappleMotor triggers it; Motor applies it.
/// </summary>
public class ImpulseCarry : NetworkBehaviour
{
    [Header("Release Boost")]
    public float releaseBoostDuration = 0.28f;
    public float releaseBoostDrag = 3.5f;

    private Vector3 _boostVel;
    private float _timer;

    [Server]
    public void ServerStart(Vector3 boostVelocity)
    {
        _boostVel = boostVelocity;
        _timer = releaseBoostDuration;
    }

    [Server]
    public void ServerApply(ref Vector3 velocity)
    {
        if (_timer <= 0f) return;

        float dt = Time.deltaTime;
        _timer -= dt;

        velocity += _boostVel * dt;

        if (releaseBoostDrag > 0f)
        {
            float k = Mathf.Clamp01(1f - releaseBoostDrag * dt);
            _boostVel *= k;
        }

        if (_timer <= 0f)
            _boostVel = Vector3.zero;
    }
}
