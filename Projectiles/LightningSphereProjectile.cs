using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSphereProjectile : MonoBehaviour
{
    private float maintain_speed = 0f; //0 for all but bounce
    private float dps;
    private int max_targets;
    private float duration;
    private float radius;
    private bool enemy_strike = false;

    public GameObject Lightning;
    public Material[] myMaterials;


    private bool[] delayed_active;
    private float time;

    private FactionLogic FL;
    private EnemyTemplateMaster ETM;


    List<Collider> TriggerList = new List<Collider>();
    struct distanceStruct
    {
        public float distance;
        public Collider collider;
    };

    List<distanceStruct> distanceList = new List<distanceStruct>();
    private GameObject[] lightningArray;
    private Transform[] lightningArrayEnding;

    private Rigidbody rb;



    static int SortByDistance(distanceStruct x0, distanceStruct x1)
    {
        return x0.distance.CompareTo(x1.distance);
    }

    private void make_circle()
    {
        LineRenderer line;
        int segments = 60;
        line = GetComponentInChildren<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;

        float angle = 0f;
        float x;
        float z;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }

        if (enemy_strike)
        {
            line.material = myMaterials[1];
        }
        else
        {
            line.material = myMaterials[0];
        }
    }


    public void EnemySetup(FactionLogic FL_in, EnemyTemplateMaster ETM_in)
    {
        FL = FL_in;
        ETM = ETM_in;
        enemy_strike = true;
    }


    public void GenericSetup(float Mainspeed, float dps_in, int mTar, float dur, float rad)
    {
        maintain_speed = Mainspeed;
        dps = dps_in;
        max_targets = mTar;
        duration = dur;
        radius = rad;

        rb = GetComponentInParent<Rigidbody>();
        make_circle();
        GetComponent<SphereCollider>().radius = radius;

        time = 0f;
        delayed_active = new bool[max_targets];
        lightningArray = new GameObject[max_targets];
        lightningArrayEnding = new Transform[max_targets];
        for (int i = 0; i < max_targets; i++)
        {
            lightningArray[i] = Instantiate(Lightning, transform.position, Quaternion.identity);
            lightningArray[i].transform.parent = gameObject.transform;

            lightningArrayEnding[i] = lightningArray[i].transform.Find("LightningEnd");
        }
    }

    void FixedUpdate() //Fixed so the damage is consistant
    {
        if (time >= duration)
        {
            Destroy(transform.parent.gameObject);
        }

        distanceList.Clear();
        distanceStruct dist;

        int potential_enemies_in_range = TriggerList.Count; //Includes recently dead enemies
        int enemies_in_range = 0;

        for (int i = potential_enemies_in_range - 1; i >= 0; i--)
        {
            Collider col = TriggerList[i];
            if (!col || (col.tag != "BasicEnemy" && col.tag != "Player")) //Remove objects from the list that were destroyed
            {
                TriggerList.Remove(col);
            }
            else
            {
                Vector3 directionToTarget = col.transform.position - transform.position;
                dist.distance = directionToTarget.magnitude;
                dist.collider = col;
                distanceList.Add(dist);
                enemies_in_range += 1;
            }
        }

        distanceList.Sort(SortByDistance);


        for (int i = 0; i < max_targets; i++) //Cannot set_active low in update then set it high
        {
            delayed_active[i] = false;
        }


        for (int i = 0; i < enemies_in_range; i++)
        {
            if (i >= max_targets)
            {
                break;
            }

            delayed_active[i] = true;
            lightningArrayEnding[i].position = distanceList[i].collider.transform.position;
            (distanceList[i].collider).GetComponent<Health>().take_damage(dps * Time.fixedDeltaTime, DamageSource.FinesseBased, !enemy_strike, DT: DamageType.Regular, isDoT: true);
        }


        for (int i = 0; i < max_targets; i++)
        {
            lightningArray[i].SetActive(delayed_active[i]);
        }


        if (rb.velocity.magnitude < maintain_speed)
        {
            float speed_multiplier = maintain_speed / rb.velocity.magnitude;
            rb.velocity = new Vector3(rb.velocity.x * speed_multiplier, 0, rb.velocity.z * speed_multiplier);
        }
        time += Time.deltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        //if the object is not already in the list
        if (!TriggerList.Contains(other))
        {
            if (enemy_strike)
            {
                if (other.tag == "Player")
                {
                    if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
                    {
                        TriggerList.Add(other);
                    }
                }
                else if (other.tag == "BasicEnemy")
                {
                    if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
                    {
                        TriggerList.Add(other);
                    }
                }
            }
            if (!enemy_strike && other.gameObject.tag == "BasicEnemy")
            {
                TriggerList.Add(other);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //if the object is in the list
        if (TriggerList.Contains(other))
        {
            if (enemy_strike)
            {
                if (other.tag == "Player")
                {
                    if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
                    {
                        TriggerList.Remove(other);
                    }
                }
                else if (other.tag == "BasicEnemy")
                {
                    if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
                    {
                        TriggerList.Remove(other);
                    }
                }
            }
            if (!enemy_strike && other.gameObject.tag == "BasicEnemy")
            {
                TriggerList.Remove(other);
            }
        }
    }
}
