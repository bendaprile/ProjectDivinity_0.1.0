using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SyntheticDemon : EnemyTemplateMaster
{
    [SerializeField] float WarpCooldown = 10f;

    [SerializeField] float projectileCooldown = 2f;
    [SerializeField] float ShieldCooldown = 8f;
    [SerializeField] int projectileDamage = 25;
    [SerializeField] Transform projectile = null;
    [SerializeField] Transform projectileSpawnPoint = null;

    private ShieldCastingEnemy SCE;
    private FactionLogic FL;

    ///////////////////////
    private float WarpRange = 20f;
    private float nextWarp = 0f;
    ///////////////////////

    ///////////////////////
    private Transform enemyProjectiles;
    private float projectileSpeed = 20f;
    private float nextFire = 0f;
    ///////////////////////

    ///////////////////////
    private float nextShield = 0f;
    ///////////////////////

    protected override void Awake()
    {
        base.Awake();
        FL = GameObject.Find("NPCs").GetComponent<FactionLogic>();
        enemyProjectiles = GameObject.Find("EnemyProjectiles").transform;
        SCE = GetComponentInChildren<ShieldCastingEnemy>();
    }

    protected override void AIFunc() //here because of no animator
    {
        if (timer > nextWarp)
        {
            if ((player.transform.position - transform.position).magnitude < 5)
            {
                Warp();
            }
        }

        if (timer > nextShield)
        {
            if ((player.transform.position - transform.position).magnitude > 5)
            {
                if (SCE.CastMechanics())
                {
                    nextShield += ShieldCooldown;
                }
            }
        }


        if (timer > nextFire)
        {
            if ((player.transform.position - transform.position).magnitude > 5)
            {
                FireProjectiles();
            }
        }

        agent.SetDestination(player.transform.position);
    }

    private void FireProjectiles()
    {
        nextFire = timer + projectileCooldown;
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.x, dir.z);
        transform.localEulerAngles = new Vector3(0f, angle * Mathf.Rad2Deg, 0f);

        float fanMod = -Mathf.PI / 12;
        for (int i = 0; i < 3; ++i)
        {
            float projectileAngle = angle + fanMod;

            Transform tempTrans = Instantiate(projectile, enemyProjectiles);

            //TODO this is garbage //Need
            //AmmoMaster AM_temp = fireProjectile(projectileAngle, true).GetComponent<AmmoMaster>();
            //AM_temp.Setup(ReturnFinalDamage(damage));
            //AM_temp.AdditionalNPCSetup(ETM.Return_FactionEnum(), FL, ETM.Return_customReputation());

            tempTrans.position = projectileSpawnPoint.position;
            tempTrans.eulerAngles = new Vector3(0f, (projectileAngle * Mathf.Rad2Deg) - 90f, 90f);
            tempTrans.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Sin(projectileAngle) * projectileSpeed, 0f, Mathf.Cos(projectileAngle) * projectileSpeed);

            fanMod += Mathf.PI / 12;
        }
    }

    private void Warp()
    {
        nextWarp = timer + WarpCooldown;
        int layerMask = (LayerMask.GetMask("Terrian") | LayerMask.GetMask("Obstacles") | LayerMask.GetMask("InteractiveThing"));
        Vector3 best = transform.position;
        for (int i = 0; i < 4; i++)
        {
            Vector3[] directions = new Vector3[] { transform.forward, transform.right, -transform.right, -transform.forward };
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directions[i], out hit, WarpRange, layerMask))
            {
                best = hit.point - directions[i];
            }
            else
            {
                best = transform.position + directions[i] * WarpRange;
                break;
            }
        }

        transform.position = best;
    }
}
