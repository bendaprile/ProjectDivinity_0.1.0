using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ColliderChild : MonoBehaviour
{
    public List<Collider> TriggerList = new List<Collider>();

    [SerializeField] private string Tag_Name = "BasicEnemy";
    [SerializeField] float eject_range = 40f;

    private SphereCollider SC;

    public void Start()
    {
        SC = GetComponent<SphereCollider>();
        if (SC)
        {
            Assert.IsTrue(eject_range == -1);
            eject_range = SC.radius * 2; //Otherwise can be instantly ejected
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!TriggerList.Contains(other))
        {
            if (other.gameObject.tag == Tag_Name)
            {
                TriggerList.Add(other);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (TriggerList.Contains(other))
        {
            if (other.gameObject.tag == Tag_Name)
            {
                TriggerList.Remove(other);
            }
        }
    }

    private int Out_of_range_iter = 0;
    private void FixedUpdate() //Remove enemies that are out of range, but didn't trigger the exit collider
    {
        if (TriggerList.Count > 0)
        {
            Out_of_range_iter %= TriggerList.Count;
            if (!TriggerList[Out_of_range_iter]) //This can happen if the enemy collider dissapears while in your radius
            {
                TriggerList.Remove(TriggerList[Out_of_range_iter]);
                return;
            }

            if ((TriggerList[Out_of_range_iter].transform.position - transform.position).magnitude > eject_range)
            {
                TriggerList.Remove(TriggerList[Out_of_range_iter]);
                return;
            }
            Out_of_range_iter += 1;
        }
    }

}
