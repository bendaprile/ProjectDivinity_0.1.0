using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardLogic : MonoBehaviour
{
    [SerializeField] Transform MidwayHostile;
    [SerializeField] GameObject Guard;

    [SerializeField] private float Shutoff_time = 300f;

    private float enable_time = 0f;

    Zone_Flags ZF;
    PlayerMaster PM;
    SphereCollider col;
    CombatChecker CC;

    private void Awake()
    {
        ZF = FindObjectOfType<Zone_Flags>();
        PM = FindObjectOfType<PlayerMaster>();
        CC = FindObjectOfType<CombatChecker>();
        col = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && Time.time > enable_time)
        {
            if (ZF.CheckFlag(MidwayHostile.name) && !CC.enemies_nearby)
            {
                enable_time = Time.time + Shutoff_time;
                Halt_Talk();
            }
        }
    }


    private void Halt_Talk()
    {
        PM.Set_PlayerControl(false);
        GameObject temp = Instantiate(Guard);

        Vector3 vec = PM.transform.position - transform.position;
        vec = (vec * (330 / col.radius)) + transform.position;

        temp.transform.position = vec;

        temp.SetActive(true);
    }
}
