using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Zone_Identifier : MonoBehaviour
{
    [SerializeField] private Transform Tundra;
    [SerializeField] private Transform Jungle;
    [SerializeField] private Transform Midway;
    [SerializeField] private Transform TotalBlockage;


    public Zones CurrentZone;
    public bool inBlockedArea;

    private List<(float, Vector3)> TotalBlockageLogic = new List<(float, Vector3)>();
    private Transform player;


    public bool inBlockedArea_point(Vector3 point)
    {
        foreach ((float, Vector3) iter in TotalBlockageLogic)
        {
            float dist = (point - iter.Item2).magnitude;
            if (dist < iter.Item1)
            {
                return true;
            }
        }
        return false;
    }


    void Start()
    {
        inBlockedArea = true; //Needed because another script can execute before Blockage check executes
        player = GameObject.Find("Player").transform;
        BlockageSetup();
    }

    private bool check_zone(Transform iter)
    {
        Vector3 vec = player.transform.position - iter.position;
        float dist = vec.magnitude;

        if(dist < iter.GetComponent<SphereCollider>().radius)
        {
            return true;
        }

        return false;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        BlockageCheck();


        if (check_zone(Tundra))
        {
            CurrentZone = Zones.Tundra;
            return;
        }

        if (check_zone(Jungle))
        {
            CurrentZone = Zones.Jungle;
            return;
        }

        if (check_zone(Midway)) //After because this is not choosen if the player is in 2 colliders
        {
            CurrentZone = Zones.Midway;
            return;
        }


        float HorDir = (player.position.x - Midway.position.x) + (Midway.position.z - player.position.z); //Not actual dist, would need to be divided by sqrt(2)

        if (HorDir > 0)
        {
            CurrentZone = Zones.Wasteland;
        }
        else
        {
            CurrentZone = Zones.Storm;
        }
    }




    private void BlockageCheck()
    {
        inBlockedArea = false;
        foreach ((float, Vector3) iter in TotalBlockageLogic)
        {
            float dist = (player.position - iter.Item2).magnitude;
            if (dist < iter.Item1)
            {
                inBlockedArea = true;
                return;
            }
        }
    }

    private void BlockageSetup()
    {
        foreach (Transform iter in TotalBlockage)
        {
            SphereCollider temp = iter.GetComponent<SphereCollider>();
            Assert.IsFalse(temp.enabled);

            TotalBlockageLogic.Add((temp.radius, iter.position));
        }
    }
}
