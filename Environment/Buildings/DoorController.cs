using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Make an empty GameObject and call it "Door"
//Drag and drop your Door model into Scene and rename it to "Body"
//Make sure that the "Door" Object is at the side of the "Body" object (The place where a Door Hinge should be)
//Move the "Body" Object inside "Door"
//Add a Collider (preferably SphereCollider) to "Door" object and make it much bigger then the "Body" model, mark it as Trigger
//Assign this script to a "Door" Object (the one with a Trigger Collider)
//Make sure the main Character is tagged "Player"
//Upon walking into trigger area press "F" to open / close the door

public class DoorController : MonoBehaviour
{
    // Smoothly open a door
    [SerializeField] protected float doorOpenAngle = 90.0f; //Set either positive or negative number to open the door inwards or outwards
    [SerializeField] protected float openSpeed = 2.0f; //Increasing this value will make the door open faster

    protected float defaultRotationAngle;
    protected float currentRotationAngle;

    [SerializeField] protected MeshRenderer mesh;
    protected Material normal_mat;
    [SerializeField] protected int matToHighlight = 0;

    protected InteractiveDoor ID;

    protected float openTime = 2;

    protected virtual void Start()
    {
        if (!mesh)
        {
            mesh = GetComponent<MeshRenderer>();
        }
        normal_mat = mesh.materials[matToHighlight];
        ID = GetComponentInParent<InteractiveDoor>();
        defaultRotationAngle = transform.localEulerAngles.y;
    }

    // Main function
    protected virtual void Update()
    {
        if (openTime < 1)
        {
            openTime += Time.deltaTime * openSpeed;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Mathf.LerpAngle(currentRotationAngle, defaultRotationAngle + (ID.DoorOpen ? doorOpenAngle : 0), openTime), transform.localEulerAngles.z);
        }
    }

    public void ActivateDoor()
    {
        currentRotationAngle = transform.localEulerAngles.y;
        openTime = 0;
    }

    public void UpdateMesh(Material Highlight)
    {
        if (Highlight)
        {
            UpdateMats(Highlight);
        }
        else
        {
            UpdateMats(normal_mat);
        }
    }

    private void UpdateMats(Material mat)
    {
        Material[] materials = mesh.materials;
        materials[matToHighlight] = mat;
        mesh.materials = materials;
    }
}
