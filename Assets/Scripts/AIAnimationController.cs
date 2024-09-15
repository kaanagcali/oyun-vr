using UnityEngine;


public class AIAnimationController : MonoBehaviour
{
    protected Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetAnimatorParameter(AnimatorParameterType parameterType, string parameterName, bool boolValue = false, float floatValue = 0f, int intValue = 0)
    {
        if (animator == null) return;
        switch (parameterType)
        {
            case AnimatorParameterType.Bool:
                animator.SetBool(parameterName, boolValue);
                break;
            case AnimatorParameterType.Float:
                animator.SetFloat(parameterName, floatValue);
                break;
            case AnimatorParameterType.Trigger:
                animator.SetTrigger(parameterName);
                break;
            case AnimatorParameterType.Int:
                animator.SetInteger(parameterName, intValue);
                break;
        }
    }

    public void SetAnimationSpeedMultiplier(float speedMultiplier)
    {
        animator.SetFloat("SpeedMultiplier", Mathf.Max(speedMultiplier, 1f));
    }

    public void SetLayerWeight(int layerIndex, float weight)
    {
        animator.SetLayerWeight(layerIndex, weight);
    }

    public void ResetAnimator()
    {
        animator.Rebind();
    }
}


public enum AnimatorParameterType
{
    Bool = 0,
    Float = 1,
    Trigger = 2,
    Int = 3,
}

public static class AnimatorParams
{
    public static string Move = "move";
    public static string MoveSpeed = "moveSpeed";
    public static string Work = "work";
    public static string WorkSpeed = "workSpeed";
    public static string Carry = "carry";
    public static string Sleep = "sleep";
    public static string Pull = "pull";
    public static string Open = "open";
    public static string Peel = "peel";
    public static string PeelSpeed = "peelSpeed";
}
