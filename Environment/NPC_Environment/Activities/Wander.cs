using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : NPC_Activity
{
    [SerializeField] float wander_x_y = 10f;
    [SerializeField] float wander_break = 3f;

    protected override float ActivityLogic(float misc_time)
    {
        if(misc_time <= Time.time)
        {
            Vector3 dest = transform.position + new Vector3(Random.Range(-wander_x_y, wander_x_y), 0, Random.Range(-wander_x_y, wander_x_y));
            CurrentNPC.npcRef.GetComponent<NPC>().Walk(dest);
            return misc_time += wander_break;
        }
        else
        {
            return -1;
        }
    }
}
