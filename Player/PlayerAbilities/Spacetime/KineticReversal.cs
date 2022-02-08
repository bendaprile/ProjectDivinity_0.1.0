using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticReversal : Ability
{
    private Transform enemyProj;
    private Transform playerProj;

    // Start is called before the first frame update
    void Start()
    {
        enemyProj = GameObject.Find("EnemyProjectiles").transform;
        playerProj = GameObject.Find("PlayerProjectiles").transform;
    }

    protected override void Attack()
    {
        for(int i = enemyProj.childCount - 1; i > 0; --i)
        {
            AmmoMaster am = enemyProj.GetChild(i).GetComponent<AmmoMaster>();
            if (am)
            {
                am.KineticReversalHelper();
                enemyProj.GetChild(i).parent = playerProj;
            }
        }
    }
}
