using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyAbility : MonoBehaviour
{
    [SerializeField] [TextArea(5, 10)] private string DevComment;

    [SerializeField] private float MaxEnemyRange = 100f;
    [SerializeField] private float RayCastheight = 1f;


    protected EnemyTemplateMaster ETM;
    protected FactionLogic FL;
    protected Transform EnemyProjectiles;


    private int Mask;
    private bool BlockingAbilityInUse = false;

    public bool return_BAiU()
    {
        return BlockingAbilityInUse;
    }

    public virtual bool CastMechanics()
    {
        Assert.IsTrue(false);
        return false;
    }

    public virtual bool CheckCast()
    {
        Assert.IsTrue(false);
        return false;
    }

    public virtual void CastMechanicsForce() //Used with CheckCast
    {
        Assert.IsTrue(false);
    }

    //////////////////////////////////////////////////////

    protected virtual void Start()
    {
        EnemyProjectiles = GameObject.Find("EnemyProjectiles").transform;
        ETM = GetComponentInParent<EnemyTemplateMaster>();
        FL = GameObject.Find("NPCs").GetComponent<FactionLogic>();

        Mask = LayerMask.GetMask("Terrain", "Obstacles", "BasicEnemy", "Player");
    }

    protected void set_BAiU(bool set)
    {
        BlockingAbilityInUse = set;
    }

    protected bool clean_LoS(bool Forward_Required = false) //Has direct Line of Sight to the player
    {
        Vector3 target_dir = ETM.Return_Current_Target().position - transform.position;
        RaycastHit hit;

        if (Forward_Required)
        {
            float target_angle = Mathf.Atan2(target_dir.x, target_dir.z) * Mathf.Rad2Deg;
            float forward_angle = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
            float diff_angle = Mathf.Abs(Mathf.DeltaAngle(target_angle, forward_angle));

            if(diff_angle > 10)
            {
                return false;
            }
        }

        Vector3 Modified_Start = new Vector3(transform.position.x, transform.position.y + RayCastheight, transform.position.z);

        Debug.DrawRay(Modified_Start, target_dir* 10f, Color.red);
        if (Physics.Raycast(Modified_Start, target_dir, out hit, MaxEnemyRange, Mask))
        {
            if (hit.collider.tag == "Player")
            {
                if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
                {
                    return true;
                }
            }
            else if(hit.collider.tag == "BasicEnemy")
            {
                if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), hit.collider.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
                {
                    return true;
                }
            }
            else
            {
                //Debug.Log((hit.collider.name, hit.collider.tag, hit.collider.gameObject.layer));
            }
        }
        return false;
    }

    protected bool target_in_range()
    {
        Vector3 distance_vector = ETM.Return_Current_Target().position - transform.position;
        return (distance_vector.magnitude < MaxEnemyRange);
    }

}
