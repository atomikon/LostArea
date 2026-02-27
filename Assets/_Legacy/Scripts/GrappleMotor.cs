using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GrappleMotor : NetworkBehaviour
{
    public enum GrappleState { None = 0, Swing = 1, Pull = 2 }

    [Header("Refs")]
    public CharacterController cc;
    public ImpulseCarry impulseCarry;

    [Header("Grapple Mask")]
    public LayerMask grappleMask = ~0;

    [Header("Auto Swing (air)")]
    public float autoSwingMaxDistance = 30f;
    public float overheadForward = 6f;
    public float overheadUp = 3.5f;
    public float overheadSearchRadius = 2.0f;
    public float minAnchorHeightAbovePlayer = 1.5f;

    [Header("Swing Physics (server)")]
    public float gravity = 25f;
    public float ropeTightness = 60f;
    public float maxSwingSpeed = 18f;

    [Header("Swing Control (server)")]
    public float swingAccel = 18f;
    public float swingDrag = 0.15f;

    [Header("Release Boost (server)")]
    public float releaseBoostSpeed = 22f;
    public float releaseBoostUp = 3.0f;
    public float releaseBoostDuration = 0.28f;
    public float releaseBoostDrag = 3.5f;

    public readonly SyncVar<GrappleState> state = new();
    public readonly SyncVar<Vector3> anchor = new();
    public readonly SyncVar<float> ropeLen = new();

    private float _swingH;
    private float _swingV;
    private float _yaw;

    private void Awake()
    {
        if (cc == null) cc = GetComponent<CharacterController>();
        if (impulseCarry == null) impulseCarry = GetComponent<ImpulseCarry>();
    }

    [Server]
    public void ServerSetInput(float h, float v, bool grappleHeld, bool grappleDown, bool grappleUp, float yaw)
    {
        _swingH = h;
        _swingV = v;
        _yaw = yaw;

        if (grappleDown && state.Value == GrappleState.None)
            TryStartAutoSwing();

        if (grappleUp)
            ServerCancel(true);

        if (!grappleHeld && state.Value != GrappleState.None)
            ServerCancel(true);
    }

    [Server]
    public void ServerApply(ref Vector3 velocity, out Vector3 positionCorrection)
    {
        positionCorrection = Vector3.zero;
        if (state.Value != GrappleState.Swing) return;

        float dt = Time.deltaTime;

        // gravity
        velocity.y -= gravity * dt;

        // control (tangential)
        Vector3 toAnchor = cc.transform.position - anchor.Value;
        float dist = toAnchor.magnitude;
        if (dist > 0.001f)
        {
            Vector3 ropeDir = toAnchor / dist;

            Vector3 fwd = Quaternion.Euler(0f, _yaw, 0f) * Vector3.forward;
            Vector3 right = Quaternion.Euler(0f, _yaw, 0f) * Vector3.right;

            Vector3 wish = (right * _swingH + fwd * _swingV);
            if (wish.sqrMagnitude > 1f) wish.Normalize();

            Vector3 tangential = wish - ropeDir * Vector3.Dot(wish, ropeDir);
            velocity += tangential * swingAccel * dt;

            // rope constraint
            float error = dist - ropeLen.Value;
            if (error > 0f)
            {
                positionCorrection = -ropeDir * error;

                float vAlong = Vector3.Dot(velocity, ropeDir);
                if (vAlong > 0f)
                    velocity -= ropeDir * vAlong;
            }
        }

        // drag + clamp
        if (swingDrag > 0f)
        {
            float k = Mathf.Clamp01(1f - swingDrag * dt);
            velocity *= k;
        }

        float sp = velocity.magnitude;
        if (sp > maxSwingSpeed && sp > 0.001f)
            velocity = velocity * (maxSwingSpeed / sp);
    }

    [Server]
    private void TryStartAutoSwing()
    {
        if (cc == null) return;

        Vector3 p = cc.transform.position;
        Vector3 origin = p + Vector3.up * overheadUp + cc.transform.forward * overheadForward;

        if (Physics.SphereCast(origin, overheadSearchRadius, Vector3.down, out RaycastHit hit, autoSwingMaxDistance, grappleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.point.y < p.y + minAnchorHeightAbovePlayer) return;

            state.Value = GrappleState.Swing;
            anchor.Value = hit.point;
            ropeLen.Value = Vector3.Distance(p, anchor.Value);
        }
    }

    [Server]
    private void ServerCancel(bool withBoost)
    {
        if (state.Value == GrappleState.None) return;

        if (withBoost && impulseCarry != null)
        {
            Vector3 v = cc != null ? cc.velocity : Vector3.zero;
            Vector3 planar = new Vector3(v.x, 0f, v.z);
            Vector3 boost = planar.normalized * releaseBoostSpeed + Vector3.up * releaseBoostUp;
            impulseCarry.releaseBoostDuration = releaseBoostDuration;
            impulseCarry.releaseBoostDrag = releaseBoostDrag;
            impulseCarry.ServerStart(boost);
        }

        state.Value = GrappleState.None;
        anchor.Value = Vector3.zero;
        ropeLen.Value = 0f;
    }
}
