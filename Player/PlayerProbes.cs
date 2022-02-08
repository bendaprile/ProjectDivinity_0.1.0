using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerProbes : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] Cameras = new CinemachineVirtualCamera[4];
    [SerializeField] private float MaxDist_normal = 18f;
    [SerializeField] private float MaxDist_combat = 28f;

    [SerializeField] private float MinDist = 5f; //if lower don't move camera
    [SerializeField] private float DistChangeMult = 1f;

    [SerializeField] private float TimeUntilTheAngleCanChangeDir = 1f;

    private CombatChecker combatChecker;
    private enum ChangeDirection { low, same, high};

    ChangeDirection AngleChange;
    float AngleChangeTimer = 0;

    private bool AdvEnabled;
    private Transform Player;

    private float[] Angles = new float[4] { 50f, 60f, 75f, 89f };
    private int layerMask;

    float final_dist = 0;
    int iter = 0;

    int[] AngleBias = new int[4] { 0, 0, -2, -4 };

    public void enableDisable(bool enable)
    {
        AdvEnabled = enable;
        if (!AdvEnabled)
        {
            ResetAdvCams();
        }
    }

    private void ResetAdvCams()
    {
        foreach (CinemachineVirtualCamera cam in Cameras)
        {
            cam.Priority = 1;
        }
    }


    void Start()
    {
        combatChecker = FindObjectOfType<CombatChecker>();
        layerMask = (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Terrain"));
        Player = GameObject.Find("Player").transform;
        final_dist = MaxDist_normal;
    }

    private void Update()
    {
        if (!AdvEnabled)
        {
            return;
        }

        float cam_dist = Cameras[iter].GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        if (cam_dist < final_dist - .1f)
        {
            Cameras[iter].GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance += DistChangeMult * (final_dist - cam_dist + 1) * Time.deltaTime;
        }
        else if (cam_dist > final_dist + .1f)
        {
            Cameras[iter].GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance -= DistChangeMult * (cam_dist - final_dist + 1) * Time.deltaTime;
        }

        ResetAdvCams();
        Cameras[iter].Priority = 10;
    }

    private bool skip_logic(int index, int prev_iter)
    {
        if(combatChecker.enemies_nearby && index == 0)
        {
            return true;
        }


        if(AngleChange == ChangeDirection.low)
        {
            //Debug.Log(("low", index, prev_iter));
            if(index > prev_iter)
            {
                return true;
            }
        }
        else if(AngleChange == ChangeDirection.high)
        {
            if (index < prev_iter)
            {
                return true;
            }
        }

        //ChangeStay is always good
        return false;
    }

    private void ChangeAngleUpdateLogic(int prev_iter)
    {
        if(prev_iter == iter && (Time.time >= AngleChangeTimer)) //Only update after timer has been reached
        {
            //Debug.Log("??");
            AngleChange = ChangeDirection.same;
        }
        else if (prev_iter > iter)
        {
            if(AngleChange == ChangeDirection.high) //Conflicting
            {
                AngleChangeTimer = Time.time + TimeUntilTheAngleCanChangeDir;
            }
            else if(AngleChangeTimer < Time.time + TimeUntilTheAngleCanChangeDir)
            {
                AngleChangeTimer = Time.time + TimeUntilTheAngleCanChangeDir;
            }
            AngleChange = ChangeDirection.low;

        }
        else if (prev_iter < iter)
        {
            if (AngleChange == ChangeDirection.high) //Conflicting
            {

            }
            else if (AngleChangeTimer < Time.time + TimeUntilTheAngleCanChangeDir)
            {

            }
            AngleChange = ChangeDirection.high;
            AngleChangeTimer = Time.time + TimeUntilTheAngleCanChangeDir;
        }
    }

    void FixedUpdate()
    {
        if (!AdvEnabled)
        {
            return;
        }

        float MaxDist;
        if (combatChecker.enemies_nearby)
        {
            MaxDist = MaxDist_combat;
        }
        else
        {
            MaxDist = MaxDist_normal;
        }

        int previous_iter = iter;
        final_dist = 0;
        for (int i = 0; i < 4; ++i)
        {
            if(skip_logic(i, previous_iter))
            {
                continue;
            }

            float y_length = Mathf.Sqrt(2) * Mathf.Tan(Angles[i] * Mathf.Deg2Rad);
            Vector3 dir = new Vector3(-Mathf.Sin(Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad), y_length, -Mathf.Cos(Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad));

            float double_dist = 0; //Average of the two distances
            for(int j = 0; j < 2; ++j)
            {
                Vector3 RayCasPos = Player.position + Vector3.up * (1 - (j * 2));
                RaycastHit hit;

                if (Physics.Raycast(RayCasPos, dir, out hit, MaxDist, layerMask))
                {
                    //Debug.Log((hit.collider.name, LayerMask.LayerToName(hit.collider.gameObject.layer)));
                    double_dist += (hit.point - RayCasPos).magnitude;
 
                }
                else
                {
                    double_dist += MaxDist;
                }
            }

            if ((double_dist + AngleBias[i]) > final_dist)
            {
                iter = i;
                final_dist = double_dist;
            }
        }

        ChangeAngleUpdateLogic(previous_iter);

        final_dist /= 2;

        if(final_dist < MinDist)
        {
            final_dist = MaxDist;
        }
    }
}
