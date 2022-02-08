using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatChecker : MonoBehaviour
{
    public bool enemies_nearby;
    public List<Collider> TriggerList;
    public List<Collider> EnemyOnlyList;

    FactionLogic FL;

    private void Start()
    {
        enemies_nearby = false;
        TriggerList = transform.GetComponent<ColliderChild>().TriggerList;
        FL = FindObjectOfType<FactionLogic>();
    }

    private void FixedUpdate()
    {
        enemies_nearby = false;
        EnemyOnlyList.Clear();

        int potential_enemies_in_range = TriggerList.Count; //Includes recently dead enemies
        for (int i = potential_enemies_in_range - 1; i >= 0; i--)
        {
            Collider col = TriggerList[i];
            if (!col || col.tag != "BasicEnemy")
            {
                TriggerList.Remove(col);
            }
            else
            {
                EnemyTemplateMaster ETM = col.GetComponentInParent<EnemyTemplateMaster>();
                if (ETM == null)
                {
                    TriggerList.Remove(col);
                }   
                else if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()) && ETM.Return_AIenabled())
                {
                    EnemyOnlyList.Add(col);
                    enemies_nearby = true;
                }
            }

        }
    }
}
