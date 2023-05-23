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
        animator.Play("CastRod");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            animator.Play("CastRod");
        }
        if(Input.GetKey(KeyCode.B))
        {
            animator.Play("Idle");
        }
        if(Input.GetKey(KeyCode.C))
        {
            animator.Play("HoldingFish");
        }
    }
}
