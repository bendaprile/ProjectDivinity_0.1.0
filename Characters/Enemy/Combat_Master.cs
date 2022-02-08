using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Circular_Element
{
    public float Valid_Dist;
    public HumanoidEnemy HE_storage;
}



public class Combat_Master : MonoBehaviour
{
    Transform Player;

    private int Circular_Iter = 0;
    private int LoS_Blockage;
    Circular_Element[] Circular_Array = new Circular_Element[36]; //0-10 //10-20 ...


    void Start()
    {
        LoS_Blockage = LayerMask.GetMask("Terrain", "Obstacles");
        Player = GameObject.Find("Player").transform;
    }

    public bool Nearest_Ranged_Position(HumanoidEnemy HE)
    {
        Vector3 Trans_m_Player = HE.transform.position - Player.position;
        float current_angle = Mathf.Atan2(Trans_m_Player.z, Trans_m_Player.x);

        if (current_angle < 0)
        {
            current_angle += (2 * Mathf.PI);
        }

        int temp_iter = Angle2Iter(current_angle, Circular_Array.Length);

        for (int i = 0; i < Circular_Array.Length; ++i)
        {
            int mod_i; //0, -1, 1, -2, 2, ...
            if(i % 2 == 0)
            {
                mod_i = (temp_iter + i / 2) % Circular_Array.Length;
            }
            else
            {
                mod_i = (temp_iter - (i + 1) / 2) % Circular_Array.Length;
            }

            if(mod_i < 0)
            {
                mod_i += Circular_Array.Length;
            }

            if (Circular_Array[mod_i].HE_storage == null && Circular_Array[mod_i].Valid_Dist >= HE.Return_Distance_Pref())
            {
                Circular_Array[mod_i].HE_storage = HE;

                float DestAngle = Iter2Angle(mod_i, Circular_Array.Length);
                Vector3 Dest = Player.position + (new Vector3(Mathf.Cos(DestAngle), 0f, Mathf.Sin(DestAngle)) * HE.Return_Distance_Pref());

                HE.ExternalCombatMovement(Dest);
                return true;
            }
        }

        Debug.Log("Failure");
        return false;
    }




    private void FixedUpdate()
    {
        Update_Circular_Array();
    }


    private float Iter2Angle(int iter, int segments)
    {
        return iter * (2 * Mathf.PI) / segments;
    }

    private int Angle2Iter(float angle, int segments)
    {
        return (int)(angle * segments / (2 * Mathf.PI));
    }


    private void Update_Circular_Array()
    {
        RaycastHit hitray;
        float angle = Iter2Angle(Circular_Iter, Circular_Array.Length);
        Vector3 Dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        

        if (Physics.Raycast(Player.position + Vector3.up, Dir, out hitray, 100f, LoS_Blockage))
        {
            Circular_Array[Circular_Iter].Valid_Dist = (hitray.point - Player.position).magnitude;
        }
        else
        {
            Circular_Array[Circular_Iter].Valid_Dist = 100f;
        }




        if(Circular_Array[Circular_Iter].HE_storage != null) //Remove from Array and re-enter if enabled
        {
            HumanoidEnemy HE_temp = Circular_Array[Circular_Iter].HE_storage;
            Circular_Array[Circular_Iter].HE_storage = null;
            if (HE_temp.Return_AIenabled() && HE_temp.gameObject.activeInHierarchy)
            {
                if(HE_temp.Return_Current_Target().tag == "Player")
                {
                    Nearest_Ranged_Position(HE_temp);
                }
                else
                {
                    HE_temp.Eject_From_Combat_System();
                }
            }
        }

        Circular_Iter = (Circular_Iter + 1) % Circular_Array.Length;
    }
}
