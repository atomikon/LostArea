using UnityEngine;

/// <summary>
/// Compatibility wrapper. Prefer using PlayerAnimatorDriver (IsSwinging).
/// </summary>
public class PlayerSwingAnimator : MonoBehaviour
{
    public Animator animator;
    public string isSwingingParam = "IsSwinging";

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void Apply(bool isSwinging)
    {
        if (animator == null) return;
        animator.SetBool(isSwingingParam, isSwinging);
    }
}
