using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TerrainLoader : MonoBehaviour
{
    [SerializeField] float terrain_size = 0;
    [SerializeField] int terrain_amount = 0;
    [SerializeField] int frames_to_wait = 1;

    private float half_terrain_size;

    private GameObject[,] terrains;


    private (bool, int, int)[] activeterrains, prevActive;

    private Transform PlayerTransform;

    private float start_counter = 0;

    private void Start()
    {
        Assert.IsTrue(frames_to_wait > 0, "Frames to wait was not greater than 0");
        half_terrain_size = terrain_size / 2;

        terrains = new GameObject[terrain_amount, terrain_amount];

        activeterrains = new (bool, int,int)[4];
        prevActive = new (bool, int, int)[4];

        PlayerTransform = GameObject.Find("Player").transform;
    }

    private void DisableTerrains()
    {
        GameObject obj;
        for (int i = 0; i < terrain_amount; i++)
        {
            for (int j = 0; j < terrain_amount; j++)
            {
                string terrain_string = "Terrain_(" + i + "," + j + ")";
                obj = GameObject.Find(terrain_string);
                if (obj != null)
                {
                    obj.SetActive(false);
                    terrains[i, j] = obj;
                }
            }
        }
    }

    private void Update()
    {
        if (start_counter < frames_to_wait) //Get to first section
        {
            start_counter += 1;
        }
    }

    private void FixedUpdate()
    {
        if (start_counter == frames_to_wait) //Ran once then go to next
        {
            DisableTerrains();
            start_counter += 1;
        }
        else if(start_counter > frames_to_wait)
        {
            UpdateTerrain();
        }
    }

    private void UpdateTerrain()
    {
        int currentX = (int)(PlayerTransform.position.x / terrain_size);
        int currentZ = (int)((terrain_size - PlayerTransform.position.z) / terrain_size);

        int subX = (int)((PlayerTransform.position.x % terrain_size) / half_terrain_size);
        int subZ = (int)(((terrain_size - PlayerTransform.position.z) % terrain_size) / half_terrain_size);

        subX = subX * 2 - 1; //-1 or 1 
        subZ = subZ * 2 - 1; //-1 or 1

        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                int modX = currentX + subX*i;//enabled or disabled ( because i is either 0 or 1)
                int modZ = currentZ + subZ*j; //enabled or disabled
                if (inbounds(modX, modZ))
                {
                    activeterrains[i + (j * 2)] = (true, modX, modZ);
                    terrains[modX, modZ].SetActive(true);
                }
                else
                {
                    activeterrains[i + (j * 2)] = (false, 0, 0);
                }

            }
        }

        for(int i = 0; i < 4; ++i)
        {
            bool Enabled = false;
            for (int k = 0; k < 4; k++)
            {
                if (activeterrains[k] == prevActive[i])
                {
                    Enabled = true;
                }
            }

            if (!Enabled && prevActive[i].Item1)
            {
                terrains[prevActive[i].Item2, prevActive[i].Item3].SetActive(false);
            }
        }

        for (int i = 0; i < 4; ++i) //Keep seperate, active terrains and prev terrains could be in different orders
        {
            prevActive[i] = activeterrains[i];
        }
    }

    bool inbounds(int x, int z)
    {
        if((x < terrain_amount && x >= 0) && (z < terrain_amount && z >= 0) && (terrains[x,z] != null))
        {
            return true;
        }
        return false;
    }

    public float GetProgress()
    {
        return start_counter / frames_to_wait;
    }

}