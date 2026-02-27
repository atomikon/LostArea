using UnityEngine;

/// <summary>
/// Compatibility wrapper. Prefer using PlayerAnimatorDriver for all params.
/// </summary>
public class PlayerWallAnimator : MonoBehaviour
{
    public Animator animator;

    public string onWallBool = "IsOnWall";
    public string wallRunBool = "IsWallRunning";
    public string wallMovingBool = "IsWallMoving";
    public string wallAttachTrigger = "WallAttach";
    public string wallDetachTrigger = "WallDetach";

    private bool _lastOnWall;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void Apply(bool onWall, bool wallRunning, bool wallMoving)
    {
        if (animator == null) return;

        animator.SetBool(onWallBool, onWall);
        animator.SetBool(wallRunBool, wallRunning);
        animator.SetBool(wallMovingBool, wallMoving);

        if (onWall && !_lastOnWall) animator.SetTrigger(wallAttachTrigger);
        if (!onWall && _lastOnWall) animator.SetTrigger(wallDetachTrigger);
        _lastOnWall = onWall;
    }
}
