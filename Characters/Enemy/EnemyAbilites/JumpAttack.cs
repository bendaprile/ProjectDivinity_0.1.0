using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class JumpAttack : EnemyAbility
{
    [SerializeField] float pre_Jump_time = .1666666f;
    [SerializeField] float Jump_time = .73333333f;
    [SerializeField] float post_Jump_time = .2f;
    [SerializeField] float damage = 40f;
    [SerializeField] GameObject External_Logic = null;
    [SerializeField] string JumpAnimation = "";
    [SerializeField] Rigidbody parent_rb = null;

    private Vector3 vel;
    private float rad;

    private Transform Indicator;
    private Transform NonLogicProjectiles;
    private Enemies_in_Range EiR;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        NonLogicProjectiles = GameObject.Find("NonLogicProjectiles").transform;

        Indicator = External_Logic.GetComponentInChildren<LineRenderer>().transform;
        EiR = External_Logic.GetComponentInChildren<Enemies_in_Range>();
        rad = EiR.GetComponent<SphereCollider>().radius;

        make_circle(); //Must be after line
        External_Logic.gameObject.SetActive(false);

        Assert.IsNotNull(External_Logic);
    }

    public override bool CheckCast()
    {
        if (!clean_LoS())
        {
            return false;
        }

        StartCoroutine(JumpMechanics());
        return true;
    }

    public void Jump_force()
    {
        StartCoroutine(JumpMechanics());
    }

    private IEnumerator JumpMechanics() //In order for animations to work, Normal movement must be disabled in the ETM
    {
        vel = (ETM.Return_Current_Target().position - transform.position) / Jump_time;
        set_BAiU(true);
        ETM.Return_EAU().PlayAnimation(JumpAnimation);

        yield return new WaitForSeconds(pre_Jump_time);

        Debug.Log(Mathf.Atan2(vel.z, vel.x));
        parent_rb.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(vel.x, vel.z) * Mathf.Rad2Deg, 0));

        Indicator.localScale = new Vector3(0, 0, 0);
        External_Logic.SetActive(true);
        External_Logic.transform.position = ETM.Return_Current_Target().position;
        External_Logic.transform.parent = NonLogicProjectiles;

        for (float i = 0; i < Jump_time; i += Time.fixedDeltaTime)
        {
            parent_rb.velocity = vel;
            yield return new WaitForFixedUpdate();
        }

        for (int i = 0; i < EiR.NearbyObjectList.Count; ++i)
        {
            EiR.NearbyObjectList[i].GetComponentInChildren<Health>().take_damage(damage, DamageSource.VigorBased);
        }

        parent_rb.velocity = vel / 2;
        External_Logic.transform.parent = transform.parent;
        External_Logic.gameObject.SetActive(false);

        yield return new WaitForSeconds(post_Jump_time);
        set_BAiU(false);
    }

    private void Update()
    {
        float expand = Indicator.localScale.x + (Time.deltaTime / Jump_time);
        Indicator.localScale = new Vector3(expand, expand, expand);
    }


    private void make_circle()
    {
        int segments = 20;
        LineRenderer line = External_Logic.GetComponentInChildren<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;

        float angle = 0f;
        float x;
        float z;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * rad;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * rad;
            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }
}
