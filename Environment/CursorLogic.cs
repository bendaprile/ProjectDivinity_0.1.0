using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLogic : MonoBehaviour
{
    private Transform playerTrans;
    private GameObject currentInteractiveThing, lastInteractiveThing;
    private Vector3 mousePos3d = new Vector3();
    private Vector3 Player2Cursor = new Vector3();

    void Start()
    {
        playerTrans = GameObject.Find("Player").GetComponent<Transform>();
        currentInteractiveThing = null;
        lastInteractiveThing = null;
    }

    public Vector3 ReturnMousePos()
    {
        return mousePos3d;
    }

    public Vector3 ReturnPlayer2Cursor()
    {
        return Player2Cursor;
    }

    // TODO: Handle for controllers
    public void InteractiveObjectFunction(bool Enabled)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = (LayerMask.GetMask("InteractiveThing"));
        RaycastHit hitray;

        lastInteractiveThing = currentInteractiveThing;

        if (Enabled && Physics.Raycast(ray, out hitray, Mathf.Infinity, layerMask))
        {
            currentInteractiveThing = hitray.collider.gameObject;
        }
        else
        {
            currentInteractiveThing = null;
        }


        if (currentInteractiveThing != lastInteractiveThing)
        {
            if (lastInteractiveThing != null) //This order is needed
            {
                lastInteractiveThing.GetComponentInParent<InteractiveThing>().CursorLeftObject();
            }

            if (currentInteractiveThing != null)
            {
                currentInteractiveThing.GetComponentInParent<InteractiveThing>().CursorOverObject();
            }
        }
    }

    // TODO: Handle for controllers
    public void GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = (LayerMask.GetMask("Terrain"));
        RaycastHit hitray;

        if (Physics.Raycast(ray, out hitray, Mathf.Infinity, layerMask))
        {
            mousePos3d = hitray.point;
        }

        Player2Cursor = mousePos3d - playerTrans.position;
    }
}
