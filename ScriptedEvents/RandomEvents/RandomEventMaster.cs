using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class RandomEventMaster : MonoBehaviour
{
    [SerializeField] private Transform Holder;

    [SerializeField] private float RE_CD = 120f;
    [SerializeField] private List<GameObject> RE_List_Input = new List<GameObject>();

    private List<GameObject>[] RE_List = new List<GameObject>[5];

    Transform player;
    Zone_Identifier ZI;

    private float RE_timer = 0f;
    GameObject enabledRE;

    void Awake()
    {
        ZI = FindObjectOfType<Zone_Identifier>();
        player = GameObject.Find("Player").transform;
        RandomEventSetup();
    }


    void FixedUpdate()
    {
        if (!enabledRE)
        {
            if (ZI.inBlockedArea) //DO NOTHING (Player in blocked area)
            {
                return;
            }
            else if (RE_timer <= 0f)
            {
                RandomEventLogic();
            }
            else
            {
                RE_timer -= Time.fixedDeltaTime;
            }
        }
        else if(enabledRE.GetComponent<RandomEvent>().Clean())
        {
            Debug.Log("Clean");
            RE_timer = RE_CD;
            enabledRE = null;
        }
    }

    public void RandomEventLogic()
    {
        int event_chooser = Random.Range(0, RE_List[(int)ZI.CurrentZone].Count);
        float enable_dist = RE_List[(int)ZI.CurrentZone][event_chooser].GetComponent<RandomEvent>().spawnDist;

        float angle = player.transform.eulerAngles.y * Mathf.Deg2Rad;
        angle += Random.Range(-Mathf.PI / 6, Mathf.PI / 6);
        Vector3 pos_pref = player.position + new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * enable_dist;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos_pref, out hit, 5, NavMesh.AllAreas)) //Search for a space within 5 meters
        {
            pos_pref = hit.position;
            //Debug.Log(pos_pref);
            if (pos_pref.y >= player.position.y + 5) //Don't let the spawn be 5 meters above the player
            {
                Debug.Log("Fail");
                return;
            }
        }
        else
        {
            return;
        }


        if (ZI.inBlockedArea_point(pos_pref)) //Do not spawn if the choosen point is in the blocked area
        {
            return;
        }

        Placer(pos_pref, event_chooser);
    }


    private void Placer(Vector3 pos, int event_iter)
    {
        enabledRE = Instantiate(RE_List[(int)ZI.CurrentZone][event_iter], Holder);
        enabledRE.transform.position = pos;
    }

    private void RandomEventSetup()
    {
        for (int i = 0; i < 5; ++i)
        {
            RE_List[i] = new List<GameObject>();
        }


        foreach(GameObject iter in RE_List_Input)
        {
            RandomEvent RE = iter.GetComponent<RandomEvent>();
            if (RE.Wasteland)
            {
                RE_List[(int)Zones.Wasteland].Add(iter);
            }
            if (RE.Storm)
            {
                RE_List[(int)Zones.Storm].Add(iter);
            }
            if (RE.Tundra)
            {
                RE_List[(int)Zones.Tundra].Add(iter);
            }
            if (RE.Jungle)
            {
                RE_List[(int)Zones.Jungle].Add(iter);
            }
            if (RE.Midway)
            {
                RE_List[(int)Zones.Midway].Add(iter);
            }
        }
    }
}
