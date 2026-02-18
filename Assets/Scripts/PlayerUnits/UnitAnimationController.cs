using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    public Animator animator;
    public bool IsAnimating { get; private set; }
    public bool IsUpdatingHealth { get; set; }

    public void PlayAttack(string triggerName)
    {
        IsAnimating = true;
        animator.SetTrigger(triggerName);
    }

    // THIS is what you're confused about calling
    public void OnAnimationFinished()
    {
        IsAnimating = false;
    }

    public void OnHitFrame()
    {
        Debug.Log("Hit frame reached");
        IsUpdatingHealth = true;
    }
}
