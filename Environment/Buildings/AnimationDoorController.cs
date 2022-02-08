using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDoorController : DoorController
{
    private Animator animator;

    protected override void Start()
    {
        if (!mesh)
        {
            mesh = GetComponent<MeshRenderer>();
        }
        normal_mat = mesh.materials[matToHighlight];
        ID = GetComponentInParent<InteractiveDoor>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (openTime < 1)
        {
            openTime += Time.deltaTime * openSpeed;

            if (ID.DoorOpen)
            {
                animator.SetFloat("Door", 1f);
            }
            else
            {
                animator.SetFloat("Door", -1f);
            }
            
        }
    }
}
