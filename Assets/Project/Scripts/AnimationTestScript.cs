using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTestScript : MonoBehaviour
{

    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            animator.Play("Idle");
        }
        if(Input.GetKey(KeyCode.Alpha2))
        {
            animator.Play("CastRod");
        }
        if(Input.GetKey(KeyCode.Alpha3))
        {
            animator.Play("Fishing");
        }
        if(Input.GetKey(KeyCode.Alpha4))
        {
            animator.Play("AlertFishing");
        }
        if(Input.GetKey(KeyCode.Alpha5))
        {
            animator.Play("PullingRod");
        }
        if(Input.GetKey(KeyCode.Alpha6))
        {
            animator.Play("ReelingRod");
        }
        if(Input.GetKey(KeyCode.Alpha7))
        {
            animator.Play("HoldingFish");
        }
    }
}
