using FishNet.Object;
using UnityEngine;

public class PlayerAnimatorDriver : NetworkBehaviour
{
    [Header("Refs")]
    public CharacterController cc;
    public Animator animator;
    public WallRunClimbMotor wallMotor;
    public GroundProbe groundProbe;
    public GrappleMotor grapple;
    public PlayerSwingAnimator swingAnim;

    [Header("Params")]
    public string moveXParam = "MoveX";
    public string moveYParam = "MoveY";
    public string speedParam = "Speed";
    public string verticalVelParam = "VerticalVel";

    public string groundedParam = "Grounded";
    public string inAirBool = "InAir";

    public string isSwingingParam = "IsSwinging";
    public string onWallBool = "IsOnWall";
    public string wallRunBool = "IsWallRunning";
    public string wallMovingBool = "IsWallMoving";

    public string jumpTrigger = "Jump";
    public string doubleJumpTrigger = "DoubleJump";
    public string landTrigger = "Land";

    [Header("Smoothing")]
    [Tooltip("Сглаживание параметров BlendTree. Чем больше — тем мягче, но тем больше инерция.")]
    public float dampTime = 0.10f;

    [Tooltip("Множитель Speed при удержании Shift (только для владельца).")]
    public float sprintAnimMultiplier = 1.5f;

    [Header("Jump Trigger (Owner)")]
    [Tooltip("Кнопка прыжка для owner-режима анимации (Input Manager).")]
    public KeyCode jumpKey = KeyCode.Space;

    private bool _prevGrounded;
    private float _prevVerticalVel;
    private int _localAirJumps;

    private int _prevNetJumpCounter;

    private void Awake()
    {
        if (cc == null) cc = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (wallMotor == null) wallMotor = GetComponent<WallRunClimbMotor>();
        if (groundProbe == null) groundProbe = GetComponent<GroundProbe>();
        if (grapple == null) grapple = GetComponent<GrappleMotor>();
        if (swingAnim == null) swingAnim = GetComponent<PlayerSwingAnimator>();
    }

    private void Update()
    {
        if (animator == null) return;

        float dt = Time.deltaTime;

        // Grounded: probe + фолбэк на cc.isGrounded, чтобы не зависать в Fall.
        bool probeGrounded = groundProbe != null && groundProbe.IsGrounded;
        bool ccGrounded = cc != null && cc.isGrounded;
        bool grounded = probeGrounded || ccGrounded;

        // Wall/Swing (оставим сетевыми)
        bool onWall = wallMotor != null && wallMotor.IsOnWall.Value;
        bool wallRun = wallMotor != null && wallMotor.IsWallRunning.Value;
        bool wallMoving = wallMotor != null && wallMotor.IsWallMoving.Value;

        bool swinging = grapple != null && grapple.state.Value == GrappleMotor.GrappleState.Swing;

        float targetMoveX;
        float targetMoveY;
        float targetSpeed;
        float verticalVel;

        if (IsOwner)
        {
            // GetAxis (не Raw) -> плавные направления.
            targetMoveX = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
            targetMoveY = Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 1f);

            float mag = Mathf.Clamp01(new Vector2(targetMoveX, targetMoveY).magnitude);

            bool sprintHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            targetSpeed = sprintHeld ? mag * sprintAnimMultiplier : mag;

            verticalVel = cc != null ? cc.velocity.y : 0f;

            // Owner jump triggers (local, instant): Jump vs DoubleJump.
            if (Input.GetKeyDown(jumpKey))
            {
                if (grounded)
                {
                    _localAirJumps = 0;
                    animator.SetTrigger(jumpTrigger);
                }
                else
                {
                    // First air press -> Jump, second -> DoubleJump.
                    if (_localAirJumps >= 1)
                        animator.SetTrigger(doubleJumpTrigger);
                    else
                        animator.SetTrigger(jumpTrigger);

                    _localAirJumps++;
                }
            }
        }
        else
        {
            targetMoveX = wallMotor != null ? wallMotor.NetMoveX.Value : 0f;
            targetMoveY = wallMotor != null ? wallMotor.NetMoveY.Value : 0f;
            targetSpeed = wallMotor != null ? wallMotor.NetSpeed.Value : 0f;
            verticalVel = wallMotor != null ? wallMotor.NetVerticalVel.Value : 0f;
        }

        animator.SetFloat(moveXParam, targetMoveX, dampTime, dt);
        animator.SetFloat(moveYParam, targetMoveY, dampTime, dt);
        animator.SetFloat(speedParam, targetSpeed, dampTime, dt);
        animator.SetFloat(verticalVelParam, verticalVel, dampTime, dt);

        animator.SetBool(groundedParam, grounded);
        animator.SetBool(inAirBool, !grounded);

        animator.SetBool(isSwingingParam, swinging);
        animator.SetBool(onWallBool, onWall);
        animator.SetBool(wallRunBool, wallRun);
        animator.SetBool(wallMovingBool, wallMoving);

        if (swingAnim != null)
            swingAnim.Apply(swinging);

        // Land trigger по приземлению
        if (!_prevGrounded && grounded)
        {
            animator.SetTrigger(landTrigger);
            _localAirJumps = 0;
        }

        // Remote jump triggers (authoritative from server): use replicated counter.
        if (!IsOwner && wallMotor != null)
        {
            int jc = wallMotor.NetJumpCounter.Value;
            if (jc != _prevNetJumpCounter)
            {
                bool isDouble = wallMotor.NetJumpIsDouble.Value;
                animator.SetTrigger(isDouble ? doubleJumpTrigger : jumpTrigger);
                _prevNetJumpCounter = jc;
            }
        }

        _prevGrounded = grounded;
        _prevVerticalVel = verticalVel;
    }
}