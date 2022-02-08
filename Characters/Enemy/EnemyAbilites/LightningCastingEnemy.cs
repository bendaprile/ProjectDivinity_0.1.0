using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LightningCastingEnemy : EnemyAbility
{
    [SerializeField] private GameObject lightningBolt = null;
    [SerializeField] private string BlockingCastAnimation = "";

    [SerializeField] private int burst_amount = 0;
    [SerializeField] private float burst_delay = 0;

    [SerializeField] private float max_radius = 0f;

    [SerializeField] private float damage = 0f;
    [SerializeField] private float AttackDelay = 0f;
    [SerializeField] private float radius = 0;


    private List<(float, float)> spots = new List<(float, float)>();

    protected override void Start() //This will generate NON-OVERLAPPING spots around the player. The strikes then choose random spots
    {
        base.Start();

        spots.Add((0, 0));

        float current_ring_radius = 2 * radius;
        while(current_ring_radius < max_radius)
        {
            float circ = 2 * Mathf.PI * current_ring_radius;
            int count = (int)(circ / (2 * radius));

            for(int i = 0; i < count; i++)
            {
                float angle = ((float)i / count) * 2 * Mathf.PI;

                spots.Add((current_ring_radius * Mathf.Sin(angle), current_ring_radius * Mathf.Cos(angle)));
            }
            current_ring_radius += 2 * radius;
        }

        Assert.IsTrue(spots.Count > burst_amount);
    }

    public override bool CastMechanics()
    {
        if (target_in_range())
        {
            //Debug.Log("cast");
            ETM.Return_EAU().PlayBlockingAnimation(BlockingCastAnimation);
            return true;
        }
        return false;
    }

    public override void CastMechanicsForce()
    {
        //Debug.Log("Start");
        StartCoroutine(attackMechanics());
    }

    private List<int> ReturnRandomNums() //Returns unique random nums
    {
        List<int> randomNums = new List<int>();
        List<int> setup_help = new List<int>();

        for(int i = 0; i < spots.Count; i++)
        {
            setup_help.Add(i);
        }


        for(int i = 0; i < burst_amount; ++i)
        {
            int val = Random.Range(0, setup_help.Count);
            randomNums.Add(setup_help[val]);
            setup_help.RemoveAt(val);
        }
        return randomNums;
    }


    private IEnumerator attackMechanics()
    {
        set_BAiU(true);
        List<int> randomNums = ReturnRandomNums();

        Vector3 enemyPos = new Vector3(ETM.Return_Current_Target().position.x, ETM.Return_Current_Target().position.y + 5, ETM.Return_Current_Target().position.z);

        for (int i = 0; i < burst_amount; i++)
        {
            Vector3 cast_pos = new Vector3(enemyPos.x + spots[randomNums[i]].Item1, enemyPos.y, enemyPos.z + spots[randomNums[i]].Item2);
            int layerMask = (LayerMask.GetMask("Terrain"));
            RaycastHit hit;

            if (Physics.Raycast(cast_pos, Vector3.down, out hit, 20, layerMask))
            {
                cast_pos = hit.point;
            }

            GameObject clone = Instantiate(lightningBolt, cast_pos, transform.rotation, EnemyProjectiles).gameObject;
            LightningBoltProjectile LBP = clone.GetComponent<LightningBoltProjectile>();
            LBP.EnemySetup(FL, ETM);
            LBP.GenericSetup(damage, radius, AttackDelay);
            yield return new WaitForSeconds(burst_delay);
        }

        set_BAiU(false);
    }
}
