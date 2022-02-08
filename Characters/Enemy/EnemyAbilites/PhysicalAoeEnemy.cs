using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PhysicalAoeEnemy : EnemyAbility
{
    [SerializeField] float warm_up = 1f;
    [SerializeField] float damage = 40f;
    [SerializeField] LineRenderer line = null;
    [SerializeField] GameObject GroundParticles = null;

    [SerializeField] string WindUpAnimation = "";

    private Transform Indicator;
    private SphereCollider SC;

    private Transform NonLogicProjectiles;

    private List<Transform> NearbyObjectList = new List<Transform>();

    protected override void Start()
    {
        base.Start();
        NonLogicProjectiles = GameObject.Find("NonLogicProjectiles").transform;
        SC = GetComponent<SphereCollider>();
        Indicator = line.transform;

        make_circle(); //Must be after line
        Indicator.gameObject.SetActive(false);

        Assert.IsNotNull(line);
        Assert.IsNotNull(SC);
    }

    public bool PhysicalAoe()
    {
        if (!target_in_range())
        {
            return false;
        }
        StartCoroutine(AoeMechanics());
        return true;
    }

    public void PhysicalAoe_force()
    {
        StartCoroutine(AoeMechanics());
    }

    private IEnumerator AoeMechanics() //In order for animations to work, Normal movement must be disabled in the ETM
    {
        set_BAiU(true);
        Indicator.gameObject.SetActive(true);
        float iter = 0;

        if (WindUpAnimation != "")
        {
            ETM.Return_EAU().PlayAnimation(WindUpAnimation);
        }

        while(iter < warm_up)
        {
            iter += Time.deltaTime;
            float expand = iter / (warm_up);
            Indicator.localScale = new Vector3(expand, expand, expand);
            yield return new WaitForEndOfFrame();
        }
        Indicator.gameObject.SetActive(false);

        for (int i = 0; i < NearbyObjectList.Count; ++i)
        {
            NearbyObjectList[i].GetComponentInChildren<Health>().take_damage(damage, DamageSource.VigorBased);
        }

        GameObject temp = Instantiate(GroundParticles, NonLogicProjectiles);
        temp.transform.position = transform.position;
        Destroy(temp, 5f);

        set_BAiU(false);
    }

    private void make_circle()
    {
        int segments = 20;
        line.positionCount = segments + 1;
        line.useWorldSpace = false;

        float angle = 0f;
        float x;
        float z;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * SC.radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * SC.radius;
            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
            {
                NearbyObjectList.Add(other.transform);
            }
        }
        else if (other.tag == "BasicEnemy")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
            {
                NearbyObjectList.Add(other.transform);
            }
        }
    }

    private int Out_of_range_iter = 0;
    private void FixedUpdate() //Remove enemies that are out of range, DOES NOT USE EXIT COLLIDER
    {
        if (NearbyObjectList.Count > 0)
        {
            Out_of_range_iter %= NearbyObjectList.Count;
            if ((NearbyObjectList[Out_of_range_iter].position - transform.position).magnitude > SC.radius)
            {
                NearbyObjectList.Remove(NearbyObjectList[Out_of_range_iter]);
            }
            Out_of_range_iter += 1;
        }
    }
}
