using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Enemies_in_Range : MonoBehaviour
{
    public List<Transform> NearbyObjectList = new List<Transform>();

    FactionLogic FL;
    EnemyTemplateMaster ETM;

    private float rad;

    void Start()
    {
        FL = FindObjectOfType<FactionLogic>();
        ETM = GetComponentInParent<EnemyTemplateMaster>();

        Assert.IsNotNull(GetComponent<SphereCollider>());
        rad = GetComponent<SphereCollider>().radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
            {
                NearbyObjectList.Add(other.transform);
            }
        }
        else if (other.tag == "BasicEnemy")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
            {
                NearbyObjectList.Add(other.transform);
            }
        }
    }

    private int Out_of_range_iter = 0;
    private void FixedUpdate() //Remove enemies that are out of range, DOES NOT USE EXIT COLLIDER
    {
        if (NearbyObjectList.Count > 0)
        {
            Out_of_range_iter %= NearbyObjectList.Count;
            if ((NearbyObjectList[Out_of_range_iter].position - transform.position).magnitude > rad)
            {
                NearbyObjectList.Remove(NearbyObjectList[Out_of_range_iter]);
            }
            Out_of_range_iter += 1;
        }
    }
}
