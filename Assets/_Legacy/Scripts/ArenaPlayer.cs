using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(WallRunClimbMotor))]
[RequireComponent(typeof(GrappleMotor))]
public class ArenaPlayer : NetworkBehaviour
{
    [Header("Refs")]
    public WallRunClimbMotor motor;
    public GrappleMotor grapple;
    public PlayerCameraRig cameraRig;
    public CombatController combat;

    private void Awake()
    {
        if (motor == null) motor = GetComponent<WallRunClimbMotor>();
        if (grapple == null) grapple = GetComponent<GrappleMotor>();
        if (combat == null) combat = GetComponent<CombatController>();
        if (cameraRig == null) cameraRig = GetComponentInChildren<PlayerCameraRig>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner && cameraRig != null)
            cameraRig.SetActive(true);

        if (!IsOwner && cameraRig != null)
            cameraRig.SetActive(false);
    }

    private void Update()
    {
        if (!IsOwner) return;

        // --- Input (keep it simple and explicit)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool sprintHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool dashDown = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
        bool slideHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        bool jumpDown = Input.GetKeyDown(KeyCode.Space);

        bool grappleDown = Input.GetKeyDown(KeyCode.Q);
        bool grappleUp = Input.GetKeyUp(KeyCode.Q);
        bool grappleHeld = Input.GetKey(KeyCode.Q);

        float yaw = (cameraRig != null) ? cameraRig.Yaw : transform.eulerAngles.y;

        // One RPC per frame is okay for arena prototype.
        SetInputServerRpc(h, v, sprintHeld, dashDown, slideHeld, jumpDown, grappleHeld, grappleDown, grappleUp, yaw);

        if (combat != null)
        {
            bool attackDown = Input.GetMouseButtonDown(0);
            bool heavyDown = Input.GetMouseButtonDown(1);
            if (attackDown) combat.RequestLightAttack();
            if (heavyDown) combat.RequestHeavyAttack();
        }
    }

    [ServerRpc]
    private void SetInputServerRpc(
        float h,
        float v,
        bool sprintHeld,
        bool dashDown,
        bool slideHeld,
        bool jumpDown,
        bool grappleHeld,
        bool grappleDown,
        bool grappleUp,
        float yaw
    )
    {
        if (motor != null)
            motor.ServerSetInput(h, v, sprintHeld, dashDown, slideHeld, jumpDown, yaw);

        if (grapple != null)
            grapple.ServerSetInput(h, v, grappleHeld, grappleDown, grappleUp, yaw);
    }
}
