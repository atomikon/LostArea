using FishNet.Object;
using UnityEngine;

/// <summary>
/// Stable grounded for CharacterController: SphereCast down + coyote time + anti-flicker.
/// Use probe.IsGrounded instead of cc.isGrounded for logic.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class GroundProbe : NetworkBehaviour
{
    public CharacterController cc;

    [Header("Ground Check")]
    public LayerMask groundMask = ~0;
    public float extraDistance = 0.15f;
    public float radiusScale = 0.95f;
    public float minGroundedTime = 0.05f;

    [Header("Coyote")]
    public float coyoteTime = 0.08f;

    public bool IsGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;

    private float _groundedHold;
    private float _coyote;

    private void Awake()
    {
        if (cc == null) cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Tick(Time.deltaTime);
    }

    public void Tick(float dt)
    {
        if (cc == null) return;

        float radius = cc.radius * radiusScale;
        Vector3 origin = cc.transform.position + cc.center + Vector3.up * 0.02f;
        float castDist = (cc.height * 0.5f) - radius + extraDistance;

        bool hitGround = Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, castDist, groundMask, QueryTriggerInteraction.Ignore);
        if (hitGround)
        {
            GroundNormal = hit.normal.sqrMagnitude < 0.5f ? Vector3.up : hit.normal.normalized;
            _groundedHold = minGroundedTime;
            _coyote = coyoteTime;
            IsGrounded = true;
            return;
        }

        if (_groundedHold > 0f)
        {
            _groundedHold -= dt;
            IsGrounded = true;
            return;
        }

        if (_coyote > 0f)
        {
            _coyote -= dt;
            IsGrounded = true;
            return;
        }

        IsGrounded = false;
        GroundNormal = Vector3.up;
    }

    public void ResetCoyote()
    {
        _coyote = 0f;
        _groundedHold = 0f;
    }
}
