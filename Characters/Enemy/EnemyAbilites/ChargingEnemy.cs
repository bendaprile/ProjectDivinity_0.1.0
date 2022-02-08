using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ChargingEnemy : EnemyAbility
{
    [SerializeField] float delay = 0;
    [SerializeField] float hitbox_width = 0;
    [SerializeField] float line_y_bias = 0;

    [SerializeField] float velocity = 0;

    [SerializeField] float duration = 0;
    [SerializeField] float damage = 0;
    [SerializeField] float ShoveForce = 0;

    [SerializeField] Rigidbody parent_rb = null;
    [SerializeField] Transform parent_transform = null;
    [SerializeField] Transform thisHitbox = null;

    private Vector3 dir;
    private GameObject line;
    private LineRenderer lr;
    private Collider sc;

    protected override void Start()
    {
        base.Start();
        Assert.IsNotNull(thisHitbox);
        line = transform.Find("Line").gameObject;
        lr = line.GetComponent<LineRenderer>();
        sc = GetComponent<Collider>();
        sc.enabled = false;
        line.SetActive(false);
        make_rect();
    }

    public bool Charge()
    {
        if (clean_LoS(true))
        {
            mod_rect(0f);
            line.SetActive(true);
            StartCoroutine(ChargeMechanics());
            return true;
        }
        return false;
    }

    private IEnumerator ChargeMechanics()
    {
        set_BAiU(true);
        float z_axis = 0;
        for (float i = 0; i < delay; i += Time.fixedDeltaTime)
        {
            z_axis += duration * velocity * Time.fixedDeltaTime / (delay);
            mod_rect(z_axis);
            dir = ETM.Return_Current_Target().position - transform.position;
            dir.y = 0;
            dir = dir.normalized;
            parent_transform.rotation = Quaternion.RotateTowards(parent_transform.rotation, Quaternion.Euler(new Vector3(0f, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, 0f)), 720 * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        RigidbodyConstraints original_constraints = parent_rb.constraints;
        parent_rb.constraints = original_constraints | RigidbodyConstraints.FreezeRotationY;

        GameObject tempInd = detachIndicator();
        line.SetActive(false);

        sc.enabled = true;
        for (float i = 0; i < duration; i += Time.fixedDeltaTime)
        {
            z_axis -= velocity * Time.fixedDeltaTime;
            //mod_rect(z_axis);
            parent_rb.velocity = dir * velocity;
            if (!clean_LoS())
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        parent_rb.constraints = original_constraints;
        sc.enabled = false;
        Destroy(tempInd);
        set_BAiU(false);
    }

    private void make_rect()
    {
        int segments = 4;
        float max_dist = velocity * duration;
        lr.positionCount = segments + 1;
        lr.useWorldSpace = false;

        lr.SetPosition(0, new Vector3(-hitbox_width / 2, line_y_bias, 0));
        lr.SetPosition(1, new Vector3(-hitbox_width / 2, line_y_bias, 0));
        lr.SetPosition(2, new Vector3(hitbox_width / 2, line_y_bias, 0));
        lr.SetPosition(3, new Vector3(hitbox_width / 2, line_y_bias, 0));
        lr.SetPosition(4, new Vector3(-hitbox_width / 2, line_y_bias, 0));
    }

    private void mod_rect(float z_axis)
    {
        lr.SetPosition(1, new Vector3(-hitbox_width / 2, line_y_bias, z_axis));
        lr.SetPosition(2, new Vector3(hitbox_width / 2, line_y_bias, z_axis));
    }

    private GameObject detachIndicator()
    {
        GameObject temp = Instantiate(line, EnemyProjectiles);
        temp.transform.position = line.transform.position;
        temp.transform.rotation = line.transform.rotation;
        return temp;
    }

    void OnTriggerEnter(Collider other) //Uses physics time
    {
        if (other.tag == "Player")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
            {
                other.GetComponent<Health>().take_damage(damage, DamageSource.VigorBased);
            }
        }
        else if (other.tag == "BasicEnemy" && other.transform != thisHitbox)
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
            {
                other.GetComponent<Health>().take_damage(damage, DamageSource.VigorBased);
            }
        }
    }

    void OnTriggerStay(Collider other) //Uses physics time
    {
        if (other.tag == "Player")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
            {
                shove(other);
            }
        }
        else if (other.tag == "BasicEnemy" && other.transform != thisHitbox)
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
            {
                shove(other);
            }
        }
    }

    private void shove(Collider other)
    {
        Vector3 Vec2Player = (other.transform.position - transform.position);
        float angle = Vector3.SignedAngle(transform.forward, Vec2Player, Vector3.up);

        Vector3 ShoveForceDir = transform.right * ShoveForce;
        if (angle < 0)
        {
            ShoveForceDir *= -1;
        }

        other.GetComponentInParent<Rigidbody>().AddForce(ShoveForceDir);
    }
}
