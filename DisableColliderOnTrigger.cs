using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableColliderOnTrigger : MonoBehaviour
{

    private Collider this_collider;

    private void Start()
    {
        this_collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && this_collider)
        {
            this_collider.enabled = false;
        }
    }
}
