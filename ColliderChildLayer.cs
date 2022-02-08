using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChildLayer : MonoBehaviour
{
    [SerializeField] float eject_range = 50f;
    public List<Collider> TriggerList = new List<Collider>();

    [SerializeField] private string LayerName = "Terrain";

    void OnTriggerEnter(Collider other)
    {
        if (!TriggerList.Contains(other))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(LayerName))
            {
                TriggerList.Add(other);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (TriggerList.Contains(other))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(LayerName))
            {
                TriggerList.Remove(other);
            }
        }
    }


    private int Out_of_range_iter = 0;
    private void FixedUpdate() //Remove enemies that are out of range, DOES NOT USE EXIT COLLIDER
    {
        if (TriggerList.Count > 0)
        {
            Out_of_range_iter %= TriggerList.Count;
            if ((TriggerList[Out_of_range_iter].transform.position - transform.position).magnitude > eject_range)
            {
                TriggerList.Remove(TriggerList[Out_of_range_iter]);
            }
            Out_of_range_iter += 1;
        }
    }
}
