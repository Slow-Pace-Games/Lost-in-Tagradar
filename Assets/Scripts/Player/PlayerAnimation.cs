using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetBool(string _name, bool _value)
    {
        animator.SetBool(_name, _value);
    }

    public void SetTrigger(string _name)
    {
        animator.SetTrigger(_name);
    }
}
