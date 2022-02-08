using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmobileNPC : EnemyTemplateMaster
{
    protected override void Awake()
    {
        timer = 0f;
        player = GameObject.Find("Player");

        AIenabled = false;
        QH = GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>();
        FL = GameObject.Find("NPCs").GetComponent<FactionLogic>();
        ZF = FindObjectOfType<Zone_Flags>();
    }


    protected override void FixedUpdate()
    {
        //Do nothing
    }
}
