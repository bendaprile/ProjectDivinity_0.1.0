using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentRandomEvents : MonoBehaviour
{
    [SerializeField] private GameObject lightningBolt = null;
    [SerializeField] private float lightningChancePerFixedUpdate = .005f;
    [SerializeField] private PlayerInRadius PlayerInLake;

    Zone_Identifier ZI;
    private Transform Player;
    private Transform EnemyProjectiles;


    private

    void Start()
    {
        ZI = FindObjectOfType<Zone_Identifier>();
        Player = GameObject.Find("Player").transform;
        EnemyProjectiles = GameObject.Find("EnemyProjectiles").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ZI.inBlockedArea)
        {
            return;
        }


        if(ZI.CurrentZone == Zones.Storm)
        {
            StormLogic();
        }
    }

    private Vector3 Y_adjust(Vector3 cast_in)
    {
        int layerMask = (LayerMask.GetMask("Terrain"));
        RaycastHit hit;
        if (Physics.Raycast(cast_in, Vector3.down, out hit, 20, layerMask))
        {
            cast_in = hit.point;
        }
        return cast_in;
    }


    private void StormLogic()
    {
        if (PlayerInLake.isTrue && Player.position.y < -1f)
        {
            float Mult_var = 1 / (1 + Mathf.Exp(Player.position.y + 4)); //S-Curve Remember player.pos will be neg near full around 8

            if (Random.value < (lightningChancePerFixedUpdate * Mult_var * 10))
            {
                Vector3 cast_pos = new Vector3(Player.position.x + Random.Range(-10, 10), Player.position.y + 5, Player.position.z + Random.Range(-10, 10));
                cast_pos = Y_adjust(cast_pos);

                GameObject clone = Instantiate(lightningBolt, cast_pos, transform.rotation, EnemyProjectiles).gameObject;
                LightningBoltProjectile LBP = clone.GetComponent<LightningBoltProjectile>();
                LBP.NatureSetup();
                LBP.GenericSetup(20 + (80 * Mult_var), 3  + (10 * Mult_var), 2);
            }
        }
        
        
        if (Random.value < lightningChancePerFixedUpdate)
        {
            Vector3 cast_pos = new Vector3(Player.position.x + Random.Range(-50, 50), Player.position.y + 5, Player.position.z + Random.Range(-50, 50));
            cast_pos = Y_adjust(cast_pos);

            GameObject clone = Instantiate(lightningBolt, cast_pos, transform.rotation, EnemyProjectiles).gameObject;
            LightningBoltProjectile LBP = clone.GetComponent<LightningBoltProjectile>();
            LBP.NatureSetup();
            LBP.GenericSetup(20, 3, 2);
        }


    }
}
