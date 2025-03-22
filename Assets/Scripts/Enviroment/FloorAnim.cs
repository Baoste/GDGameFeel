using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorAnim : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void AnimationTrigger()
    {
        animator.SetBool("isPlay", false);
        transform.localRotation = Quaternion.identity;
    }
}
