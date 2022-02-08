using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEvent : MonoBehaviour
{
    [SerializeField] public bool Wasteland;
    [SerializeField] public bool Storm;
    [SerializeField] public bool Tundra;
    [SerializeField] public bool Jungle;
    [SerializeField] public bool Midway;

    [SerializeField] public float spawnDist = 45f;
    [SerializeField] Transform transformGlue; //Used so that the event isn't over until the player is away from this object

    Spawner spawn;
    private Transform player;

    private const float disableDist = 150f;

    private void Start()
    {
        player = GameObject.Find("Player").transform;

        if (transformGlue)
        {
            transformGlue.gameObject.SetActive(true);
        }

        spawn = GetComponent<Spawner>();
        if (spawn)
        {
            spawn.External_Initalize(true);
        }
    }

    public bool Clean()
    {
        if (transformGlue)
        {
            float RE_dist = (transformGlue.position - player.position).magnitude;
            if(RE_dist >= disableDist)
            {
                if(!transformGlue.GetComponent<EnemyTemplateMaster>().Return_AIenabled() || !transformGlue.gameObject.activeInHierarchy)
                {
                    transformGlue.GetComponent<EnemyTemplateMaster>().Death(true); //Attempt to kill non-Active
                    Destroy(gameObject);
                    return true;
                }
            }
            else if (transformGlue.GetComponent<EnemyTemplateMaster>().isDead())
            {
                Destroy(gameObject);
                return true;
            }
        }

        if (spawn)
        {
            float RE_dist = (transform.position - player.position).magnitude; //Distance from spawner
            if (RE_dist >= disableDist)
            {
                if (spawn.KillSpawnerChildren(true)) //Attempt to kill non-active enemies
                {
                    Destroy(gameObject);
                    return true;
                }
            }
            else if(spawn.Return_EnemyCount() == 0)
            {
                Destroy(gameObject);
                return true;
            }
        }

        return false;
    }
}
