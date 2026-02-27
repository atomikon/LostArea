using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WallRunClimbMotor : NetworkBehaviour
{
    [Header("Setup")]
    public CharacterController cc;
    public GroundProbe groundProbe;
    public LayerMask wallMask = ~0;

    [Header("Optional Refs")]
    public StaminaComponent stamina;
    public PlayerWallAnimator wallAnim;
    public GrappleMotor grapple;
    public ImpulseCarry impulseCarry;

    [Header("Move")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 8f;
    public float gravity = 25f;
    public float jumpHeight = 3.2f;
    [Tooltip("Макс. количество прыжков (2 = двойной прыжок).")]
    public int maxJumps = 2;
    [Tooltip("Множитель высоты для второго прыжка (1 = такая же высота).")]
    public float doubleJumpHeightMultiplier = 0.95f;
    public float airControl = 8f;

    [Header("Dash (Ctrl tap)")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.6f;
    public float dashCost = 18f;

    [Header("Slide (Alt hold)")]
    public float slideSpeed = 11f;
    public float slideDuration = 0.35f;
    public float slideCooldown = 0.6f;
    public float slideCost = 14f;

    [Header("Wall Run / Climb")]
    public float probeHeight = 1.2f;
    public float probeRadius = 0.35f;
    public float probeDistance = 0.60f;
    public float maxWallNormalY = 0.25f;

    public float climbUpSpeed = 3.0f;
    public float climbDownSpeed = 2.0f;
    public float wallRunUpSpeed = 7.5f;
    public float wallSideSpeed = 5.0f;

    [Header("Idle On Wall")]
    public float wallIdleDownSpeed = 0.25f;

    [Header("Stick")]
    public float stickForce = 5.0f;
    public float releaseCooldown = 0.15f;

    [Header("Wall Jump")]
    public float wallJumpUp = 8.0f;
    public float wallJumpOut = 6.0f;

    [Header("Stamina (only for wallRun / Shift)")]
    public float wallRunStaminaPerSecond = 22f;
    public float minStaminaToStartWallRun = 5f;

    public readonly SyncVar<bool> IsOnWall = new();
    public readonly SyncVar<bool> IsWallRunning = new();
    public readonly SyncVar<bool> IsWallMoving = new();

    public readonly SyncVar<float> NetMoveX = new();
    public readonly SyncVar<float> NetMoveY = new();
    public readonly SyncVar<float> NetSpeed = new();
    public readonly SyncVar<float> NetVerticalVel = new();

    // Jump replication for animations (owner + remote).
    public readonly SyncVar<int> NetJumpCounter = new();
    public readonly SyncVar<bool> NetJumpIsDouble = new();

    private Vector3 _velocity;
    private int _jumpsUsed;

    private Vector3 _wallNormal;
    private float _releaseTimer;

    private float _dashTimer;
    private float _dashCooldownTimer;

    private float _slideTimer;
    private float _slideCooldownTimer;

    private float _moveX;
    private float _moveY;
    private bool _sprintHeld;
    private bool _dashDown;
    private bool _slideHeld;
    private bool _jumpDown;
    private float _yaw;

    private void Awake()
    {
        if (cc == null) cc = GetComponent<CharacterController>();
        if (groundProbe == null) groundProbe = GetComponent<GroundProbe>();
        if (stamina == null) stamina = GetComponent<StaminaComponent>();
        if (grapple == null) grapple = GetComponent<GrappleMotor>();
        if (impulseCarry == null) impulseCarry = GetComponent<ImpulseCarry>();
        if (wallAnim == null) wallAnim = GetComponent<PlayerWallAnimator>();
    }

    private void Update()
    {
        if (!IsServer) return;
        ServerTick();
    }

    [Server]
    public void ServerSetInput(float h, float v, bool sprintHeld, bool dashDown, bool slideHeld, bool jumpDown, float yaw)
    {
        _moveX = Mathf.Clamp(h, -1f, 1f);
        _moveY = Mathf.Clamp(v, -1f, 1f);
        _sprintHeld = sprintHeld;
        _dashDown = dashDown;
        _slideHeld = slideHeld;
        _jumpDown = jumpDown;
        _yaw = yaw;
    }

    private void ServerTick()
    {
        float dt = Time.deltaTime;

        bool grounded = groundProbe != null ? groundProbe.IsGrounded : cc.isGrounded;

        bool swinging = (grapple != null && grapple.state.Value == GrappleMotor.GrappleState.Swing);

        // --- Gravity baseline (skip when swinging: GrappleMotor applies gravity)
        if (!swinging)
            _velocity.y -= gravity * dt;

        // --- Movement input in camera yaw space
        Vector3 input = new Vector3(_moveX, 0f, _moveY);
        input = Vector3.ClampMagnitude(input, 1f);

        Quaternion yawRot = Quaternion.Euler(0f, _yaw, 0f);
        Vector3 wishDir = yawRot * input;

        float targetSpeed = _sprintHeld ? sprintSpeed : walkSpeed;
        Vector3 targetPlanar = wishDir * targetSpeed;

        // --- Wall logic
        bool wallOK = TryProbeWall(out Vector3 wallNormal);
        if (!wallOK)
        {
            if (_releaseTimer > 0f) _releaseTimer -= dt;
            else SetWallState(false, false, false, Vector3.zero);
        }
        else
        {
            _wallNormal = wallNormal;
        }

        // Wall Jump (если на стене и нажал прыжок)
        if (_jumpDown && IsOnWall.Value)
        {
            if (TryWallJump(ref _velocity, _wallNormal))
            {
                // Wall jump засчитываем как первый прыжок -> остаётся второй.
                _jumpsUsed = Mathf.Max(_jumpsUsed, 1);

                // Replicate jump event for animations.
                NetJumpCounter.Value++;
                NetJumpIsDouble.Value = false;
            }
        }

        // Grounded reset
        if (grounded)
        {
            if (_velocity.y < 0f) _velocity.y = -2f; // keep grounded
            _jumpsUsed = 0;
        }

        // Jump / Double-jump
        if (_jumpDown)
        {
            if (_jumpsUsed < Mathf.Max(1, maxJumps))
            {
                float mul = (_jumpsUsed == 0) ? 1f : doubleJumpHeightMultiplier;
                float jumpVel = Mathf.Sqrt(2f * gravity * jumpHeight * mul);

                if (_velocity.y < 0f) _velocity.y = 0f;
                _velocity.y = jumpVel;
                _jumpsUsed++;

                // Replicate jump event for animations.
                NetJumpCounter.Value++;
                NetJumpIsDouble.Value = (_jumpsUsed >= 2);

                if (grounded && groundProbe != null)
                    groundProbe.ResetCoyote();
            }
        }

        // ---- Dash
        if (_dashDown && grounded && _dashCooldownTimer <= 0f)
        {
            if (stamina == null || stamina.value.Value >= dashCost)
            {
                if (stamina != null) stamina.value.Value -= dashCost;
                _dashTimer = dashDuration;
                _dashCooldownTimer = dashCooldown;
            }
        }
        if (_dashCooldownTimer > 0f) _dashCooldownTimer -= dt;

        // ---- Slide
        if (_slideHeld && grounded && _slideCooldownTimer <= 0f)
        {
            if (_slideTimer <= 0f)
            {
                if (stamina == null || stamina.value.Value >= slideCost)
                {
                    if (stamina != null) stamina.value.Value -= slideCost;
                    _slideTimer = slideDuration;
                    _slideCooldownTimer = slideCooldown;
                }
            }
        }
        if (_slideCooldownTimer > 0f) _slideCooldownTimer -= dt;

        // ---- Apply planar
        Vector3 planarVel = new Vector3(_velocity.x, 0f, _velocity.z);

        if (IsOnWall.Value)
        {
            // Wall run / climb behavior
            bool moving = input.sqrMagnitude > 0.001f;
            bool wantWallRun = _sprintHeld && moving;

            if (wantWallRun)
            {
                // stamina drain
                if (stamina != null)
                {
                    if (stamina.value.Value <= minStaminaToStartWallRun)
                        wantWallRun = false;
                    else
                        stamina.value.Value -= wallRunStaminaPerSecond * dt;
                }
            }

            if (wantWallRun)
            {
                SetWallState(true, true, true, _wallNormal);

                // Up or side
                Vector3 alongWall = Vector3.Cross(Vector3.up, _wallNormal).normalized;
                float forward = Vector3.Dot(wishDir, alongWall);
                if (Mathf.Abs(forward) > 0.2f)
                    planarVel = alongWall * (Mathf.Sign(forward) * wallSideSpeed);
                else
                    planarVel = wishDir.normalized * wallRunUpSpeed;

                // reduce gravity feeling while wallrunning
                _velocity.y = Mathf.Max(_velocity.y, -2.5f);

                // stick to wall
                planarVel += (-_wallNormal * stickForce);
            }
            else if (moving)
            {
                // climbing up/down without sprint
                SetWallState(true, false, true, _wallNormal);

                float vy = (input.z > 0.2f) ? climbUpSpeed : (input.z < -0.2f ? -climbDownSpeed : -wallIdleDownSpeed);
                _velocity.y = vy;

                planarVel = (-_wallNormal * stickForce);
            }
            else
            {
                // idle on wall
                SetWallState(true, false, false, _wallNormal);
                _velocity.y = -wallIdleDownSpeed;
                planarVel = (-_wallNormal * stickForce);
            }
        }
        else
        {
            // Air control / grounded control
            float accel = grounded ? 20f : airControl;
            planarVel = Vector3.MoveTowards(planarVel, targetPlanar, accel * dt);

            // Slide overrides planar
            if (_slideTimer > 0f)
            {
                _slideTimer -= dt;
                Vector3 forward = yawRot * Vector3.forward;
                planarVel = forward * slideSpeed;
            }

            // Dash overrides planar
            if (_dashTimer > 0f)
            {
                _dashTimer -= dt;
                Vector3 forward = yawRot * Vector3.forward;
                planarVel = forward * dashSpeed;
            }
        }

        _velocity.x = planarVel.x;
        _velocity.z = planarVel.z;

        // Impulse carry (optional): server-side boost after grapple release.
        if (impulseCarry != null)
            impulseCarry.ServerApply(ref _velocity);

        // Grapple (optional): swing physics + rope constraint correction.
        Vector3 posCorr = Vector3.zero;
        if (grapple != null)
            grapple.ServerApply(ref _velocity, out posCorr);

        // Apply move
        if (posCorr.sqrMagnitude > 0f)
            cc.Move(posCorr);

        cc.Move(_velocity * dt);

        // Sync for others
        NetMoveX.Value = _moveX;
        NetMoveY.Value = _moveY;
        NetSpeed.Value = Mathf.Clamp01(input.magnitude) * (_sprintHeld ? 1.5f : 1.0f);
        NetVerticalVel.Value = _velocity.y;
    }

    private bool TryProbeWall(out Vector3 wallNormal)
    {
        wallNormal = Vector3.zero;

        if (_releaseTimer > 0f)
            return false;

        Vector3 origin = transform.position + Vector3.up * probeHeight;
        float radius = probeRadius;

        // probe in move direction first, otherwise forward
        Vector3 dir = new Vector3(_velocity.x, 0f, _velocity.z);
        if (dir.sqrMagnitude < 0.01f)
            dir = transform.forward;
        dir.Normalize();

        bool hit = Physics.SphereCast(origin, radius, dir, out RaycastHit rh, probeDistance, wallMask, QueryTriggerInteraction.Ignore);
        if (!hit) return false;

        Vector3 n = rh.normal.normalized;
        if (n.y > maxWallNormalY) return false;

        wallNormal = n;
        SetWallState(true, IsWallRunning.Value, IsWallMoving.Value, wallNormal);
        return true;
    }

    private bool TryWallJump(ref Vector3 velocity, Vector3 wallNormal)
    {
        // If not on wall, no wall jump
        if (!IsOnWall.Value)
            return false;

        // apply cooldown to avoid instant re-stick
        _releaseTimer = releaseCooldown;
        velocity = Vector3.up * wallJumpUp + wallNormal * wallJumpOut;
        _jumpsUsed = Mathf.Max(_jumpsUsed, 1);
        SetWallState(false, false, false, Vector3.zero);
        return true;
    }

    private void SetWallState(bool onWall, bool wallRunning, bool wallMoving, Vector3 wallNormal)
    {
        IsOnWall.Value = onWall;
        IsWallRunning.Value = wallRunning;
        IsWallMoving.Value = wallMoving;

        if (wallAnim != null)
            wallAnim.Apply(onWall, wallRunning, wallMoving);
    }
}