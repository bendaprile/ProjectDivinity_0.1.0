using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LightningBoltProjectile: MonoBehaviour
{
    [SerializeField] GameObject GroundEffect;

    private float AttackDelay;
    private float damage; //Ticks 13 times at .25
    private float radius;
    private bool enemy_strike = false;
    private bool nature_strike = false;
    public Material[] myMaterials;

    private GameObject preattack_loc;
    private GameObject attack_loc;


    private float real_volume;
    private bool audio_ending;
    private AudioSource audio_var;

    private bool deal_damage;

    private LineRenderer line;

    private FactionLogic FL;
    private EnemyTemplateMaster ETM;


    public void NatureSetup()
    {
        nature_strike = true;
    }

    public void EnemySetup(FactionLogic FL_in, EnemyTemplateMaster ETM_in)
    {
        enemy_strike = true;
        FL = FL_in;
        ETM = ETM_in;
    }

    public void GenericSetup(float dam_in, float rad_in, float del_in)
    {
        damage = dam_in;
        radius = rad_in;
        AttackDelay = del_in;

        audio_ending = false;
        audio_var = this.GetComponent<AudioSource>();
        real_volume = audio_var.volume;

        make_circle();

        attack_loc = transform.Find("Lightning Bolt Attack").gameObject;
        preattack_loc = transform.Find("Lightning Bolt preAttack").gameObject;

        GetComponent<SphereCollider>().radius = radius;

        attack_loc.SetActive(false);
        deal_damage = false;

        StartCoroutine(attack());
    }
    

    private void make_circle()
    {
        int segments = 20;
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
            line.SetPosition(i,new Vector3(x, 0, z));

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


    private IEnumerator attack()
    {
        audio_var.enabled = true;
        audio_var.time = 6 - (AttackDelay);

        yield return new WaitForSeconds(AttackDelay);

        attack_loc.SetActive(true);
        deal_damage = true;
        preattack_loc.SetActive(false);
        GameObject Temp = Instantiate(GroundEffect, GameObject.Find("NonLogicProjectiles").transform);
        Temp.transform.position = transform.position;
        Destroy(Temp, 5);

        yield return new WaitForSeconds(.25f); //Duration

        deal_damage = false;
        attack_loc.SetActive(false);
        audio_ending = true;

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }

    private void Update()
    {
        float expand = 1 / (AttackDelay);

        preattack_loc.transform.localScale += new Vector3(expand, expand, expand) * Time.deltaTime;

        if (audio_ending)
        {
            audio_var.volume -= real_volume * Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider other) //Uses physics time //TODO CHANGE TO SUPPORT 
    {
        if (deal_damage)
        {
            if (enemy_strike)
            {
                if (other.tag == "Player")
                {
                    if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
                    {
                        other.gameObject.GetComponent<Health>().take_damage(damage * 4 * Time.fixedDeltaTime, DamageSource.FinesseBased, DT: DamageType.Regular, isDoT: true);
                    }
                }
                else if (other.tag == "BasicEnemy")
                {
                    if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
                    {
                        other.gameObject.GetComponent<Health>().take_damage(damage * 4 * Time.fixedDeltaTime, DamageSource.FinesseBased, DT: DamageType.Regular, isDoT: true);
                    }
                }
            }

            if (!enemy_strike && other.gameObject.tag == "BasicEnemy")
            {
                other.gameObject.GetComponent<Health>().take_damage(damage * 4 * Time.fixedDeltaTime, DamageSource.FinesseBased, true, DT: DamageType.Regular, isDoT: true);
            }

            if (nature_strike && other.tag == "Player") //Enemies will be damaged right above
            {
                other.gameObject.GetComponent<Health>().take_damage(damage * 4 * Time.fixedDeltaTime, DamageSource.FinesseBased, DT: DamageType.Regular, isDoT: true);
            }
        }
    }
}
