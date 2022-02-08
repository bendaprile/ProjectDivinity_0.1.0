using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidMeleeBox : MonoBehaviour
{
    [SerializeField] float eject_range = 5f;
    private HumanoidMaster HM;
    private FactionLogic FL;
    private EnemyTemplateMaster ETM;
    public List<Transform> TargetsInMeleeRange = new List<Transform>();


    protected virtual void Start()
    {
        HM = GetComponentInParent<HumanoidMaster>();
        ETM = GetComponentInParent<EnemyTemplateMaster>();
        FL = GameObject.Find("NPCs").GetComponent<FactionLogic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
            {
                TargetsInMeleeRange.Add(other.transform);
            }
        }
        else if (other.tag == "BasicEnemy")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
            {
                TargetsInMeleeRange.Add(other.transform);
            }
        }
    }

    private int Out_of_range_iter = 0;
    private void FixedUpdate() //Remove enemies that are out of range, but didn't trigger the exit collider
    {
        if (TargetsInMeleeRange.Count > 0)
        {
            Out_of_range_iter %= TargetsInMeleeRange.Count;
            if ((TargetsInMeleeRange[Out_of_range_iter].position - transform.position).magnitude > eject_range)
            {
                TargetsInMeleeRange.Remove(TargetsInMeleeRange[Out_of_range_iter]);
            }
            Out_of_range_iter += 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
            {
                TargetsInMeleeRange.Remove(other.transform);
            }
        }
        else if (other.tag == "BasicEnemy" && other.transform != HM.Return_selfTrans())
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
            {
                TargetsInMeleeRange.Add(other.transform);
            }
        }
    }
}
