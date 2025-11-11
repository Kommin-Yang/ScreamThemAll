using UnityEngine;

public class HumanBehavior : MonoBehaviour
{
    private Animator animator;

    private CapsuleCollider capsuleCollider;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (capsuleCollider == null)
            capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void Scared()
    {
        if (capsuleCollider)
        {
            capsuleCollider.enabled = false;
        }
        animator.Play("GetHit");
    }

    public void ResetPeasant()
    {
        if (capsuleCollider)
        {
            capsuleCollider.enabled = true;
        }
        animator.Play("Idle");
    }
}