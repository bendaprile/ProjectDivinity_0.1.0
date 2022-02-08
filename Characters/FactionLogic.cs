using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionLogic : MonoBehaviour
{
    private Transform[] Tranform_Holder = new Transform[STARTUP_DECLARATIONS.FactionCount];
    private float[][] ReputationMatrix = new float[STARTUP_DECLARATIONS.FactionCount][];

    [SerializeField] private GameObject[] HostileFlags = new GameObject[STARTUP_DECLARATIONS.FactionCount];

    private Zone_Flags ZF;

    private bool matrix_set = false;


    private void Start()
    {
        ZF = FindObjectOfType<Zone_Flags>();

        if (!matrix_set) //Don't set if loading
        {
            matrix_set = true;
            for (int i = 0; i < STARTUP_DECLARATIONS.FactionCount; i++)
            {
                Modify_Reputation(FactionsEnum.Player, (FactionsEnum)i, 0);
            }
        }
    }


    public float[][] return_fullMatrix()
    {
        return ReputationMatrix;
    }
    
    public void set_fullMatrix(float[][] matrix_in)
    {
        ReputationMatrix = matrix_in;
    }


    //Goes from 0 to 2000
    //Under 200 means they are your enemy
    //Over 1800 means they are your ally

    public float[][] return_ReputationMatrix()
    {
        return ReputationMatrix;
    }



    public void Modify_Reputation(FactionsEnum FactionA, FactionsEnum FactionB, float ModifyValue)
    {
        int A = (int)FactionA;
        int B = (int)FactionB;

        if(A < 5 && B < 5) //Cannot modify these
        {
            return;
        }

        if (A > B)
        {
            int C = A;
            A = B;
            B = C;
        }
        B -= A;

        ReputationMatrix[A][B] += ModifyValue;
        if(ReputationMatrix[A][B] < 0)
        {
            ReputationMatrix[A][B] = 0;
        }
        else if(ReputationMatrix[A][B] > 2000)
        {
            ReputationMatrix[A][B] = 2000;
        }

        if (ReputationMatrix[A][B] < STARTUP_DECLARATIONS.EnemyNumber) //Set hostile
        {
            if (FactionA == FactionsEnum.Player)
            {
                if (HostileFlags[(int)FactionB])
                {
                    ZF.SetFlag(HostileFlags[(int)FactionB]);
                }
            }

            if (FactionB == FactionsEnum.Player)
            {
                if (HostileFlags[(int)FactionA])
                {
                    ZF.SetFlag(HostileFlags[(int)FactionA]);
                }
            }
        }
    }

    public bool ReturnIsEnemy(FactionsEnum Fac_caster, FactionsEnum Fac_target, CustomReputation CustomMod)
    {
        if(CustomMod == CustomReputation.PlayerEnemy && (Fac_caster == FactionsEnum.Player || Fac_target == FactionsEnum.Player))
        {
            return true;
        }
        else
        {
            return AccessReputationMatrix((int)Fac_caster, (int)Fac_target) <= STARTUP_DECLARATIONS.EnemyNumber;
        }
    }

    public bool ReturnIsAlly(FactionsEnum Fac_caster, FactionsEnum Fac_target)
    {
        return AccessReputationMatrix((int)Fac_caster, (int)Fac_target) >= STARTUP_DECLARATIONS.AllyNumber;
    }

    public Transform ReturnAlly_withinDistance(FactionsEnum fac_in, Transform thisTransform, float MaxDistance)
    {
        for (int i = 0; i < STARTUP_DECLARATIONS.FactionCount; ++i) //DOES NOT INCLUDE THE PLAYER
        {
            if ((FactionsEnum)i == FactionsEnum.Player)
            {
                continue;
            }

            if (ReturnIsAlly(fac_in, (FactionsEnum)i))
            {
                foreach (Transform trans in Tranform_Holder[i])
                {
                    EnemyTemplateMaster iter_ETM = trans.GetComponent<EnemyTemplateMaster>();
                    if (trans.gameObject.activeSelf && (trans != thisTransform) && iter_ETM.Return_AIenabled()) //enabled and AIenabled (DIFFERENT FROM THE REST)
                    {
                        float dist = (trans.position - thisTransform.position).magnitude;
                        if (dist < MaxDistance)
                        {
                            return trans;
                        }
                    }
                }
            }
        }
        return null;
    }

    public void RallyAllies(FactionsEnum fac_in, Transform thisTransform, CustomReputation CustomRep, float EnableRange = 100)
    {
        for (int i = 0; i < STARTUP_DECLARATIONS.FactionCount; ++i) //DOES NOT INCLUDE THE PLAYER
        {
            if((FactionsEnum)i == FactionsEnum.Player)
            {
                continue;
            }

            if (ReturnIsAlly(fac_in, (FactionsEnum)i))
            {
                //Debug.Log("AllyFound");
                foreach (Transform trans in Tranform_Holder[i])
                {
                    EnemyTemplateMaster iter_ETM = trans.GetComponent<EnemyTemplateMaster>();

                    //Debug.Log((trans.gameObject.activeSelf, (trans != thisTransform), !iter_ETM.Return_AIenabled()));

                    if (trans.gameObject.activeSelf && (trans != thisTransform) && !iter_ETM.Return_AIenabled()) //enabled, but not AIenabled
                    {
                        float dist = (trans.position - thisTransform.position).magnitude;
                        if (dist < EnableRange)
                        {
                            trans.GetComponentInChildren<EnemyTemplateMaster>().Set_customReputation(CustomRep);
                            trans.GetComponentInChildren<EnemyTemplateMaster>().EnableAI(true);
                        }
                    }
                }
            }
        }
    } //Meant for one big enable

    float Last_Time_Used = 0.0f;
    public (bool, bool) RallyAlliesLoS(FactionsEnum fac_in, Transform thisTransform, CustomReputation CustomRep, float y_offset = 1f, float EnableRange = 40) //Only allow one per frame
    {
        if(Time.time == Last_Time_Used) //Used this frame
        {
            return (false, false); // Tested / Ally found
        }

        Last_Time_Used = Time.time;
        bool ally_found = false;

        int layerMask = LayerMask.GetMask("BasicEnemy", "Obstacles", "Terrain");

        for (int i = 0; i < STARTUP_DECLARATIONS.FactionCount; ++i) //DOES NOT INCLUDE THE PLAYER
        {
            if ((FactionsEnum)i == FactionsEnum.Player)
            {
                continue;
            }

            if (ReturnIsAlly(fac_in, (FactionsEnum)i))
            {
                foreach (Transform trans in Tranform_Holder[i])
                {
                    EnemyTemplateMaster iter_ETM = trans.GetComponent<EnemyTemplateMaster>();

                    if (trans.gameObject.activeSelf && (trans != thisTransform) && !iter_ETM.Return_AIenabled()) //enabled, but not AIenabled
                    {
                        Vector3 Dir = (trans.position - thisTransform.position);
                        RaycastHit hit;
                        if (Physics.Raycast(thisTransform.position + Vector3.up * y_offset, Dir, out hit, EnableRange, layerMask))
                        {
                            if(hit.transform == trans)
                            {
                                ally_found = true;
                                trans.GetComponentInChildren<EnemyTemplateMaster>().Set_customReputation(CustomRep);
                                trans.GetComponentInChildren<EnemyTemplateMaster>().EnableAI(true);
                            }
                        }
                    }
                }
            }
        }
        return (true, ally_found);
    }

    public Transform FindEnemy(FactionsEnum fac_in, Transform thisTransform, CustomReputation CustomMod, float FindRange = 100)
    {
        Transform ClosestEnemy = null;
        float min_dist = FindRange;

        for (int i = 0; i < STARTUP_DECLARATIONS.FactionCount; ++i)
        {
            if (ReturnIsEnemy(fac_in, (FactionsEnum)i, CustomMod))
            {
                foreach (Transform trans in Tranform_Holder[i])
                {
                    if (trans.gameObject.activeSelf && (trans != thisTransform)) //enabled
                    {
                        float dist = (trans.position - thisTransform.position).magnitude;
                        if (dist < min_dist) //Need to check because rogues will attack all
                        { 
                            min_dist = dist;
                            ClosestEnemy = trans;
                        }
                    }
                }
            }
        }

        return ClosestEnemy;
    }

    private void Awake()
    {
        for (int i = 0; i < STARTUP_DECLARATIONS.FactionCount; ++i)
        {
            Tranform_Holder[i] = transform.Find(STARTUP_DECLARATIONS.FactionsEnumReverse[i]);
        }

        SetupReputationMatrix();
    }

    private float AccessReputationMatrix(int A, int B)
    {
        if (A > B)
        {
            int C = A;
            A = B;
            B = C;
        }

        B -= A;

        return ReputationMatrix[A][B];
    }


    private void SetupReputationMatrix()
    {
        ReputationMatrix[(int)FactionsEnum.Neutral] = new float[STARTUP_DECLARATIONS.FactionCount]
        {
           1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000
        };
        ReputationMatrix[(int)FactionsEnum.Player] = new float[STARTUP_DECLARATIONS.FactionCount - 1]
        {
            2000, 0, 0, 0, 1000, 1000, 1000, 1000, 1000, 1000, 1000

            //2000, 0, 0, 0, 1000, 1000, 1000, 0, 1000, 1000 // Hostile Midway
        };
        ReputationMatrix[(int)FactionsEnum.Rogue] = new float[STARTUP_DECLARATIONS.FactionCount - 2]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };
        ReputationMatrix[(int)FactionsEnum.Feral] = new float[STARTUP_DECLARATIONS.FactionCount - 3]
        {
            2000, 0, 0, 0, 0, 0, 0, 0, 0
        };
        ReputationMatrix[(int)FactionsEnum.AntiPlayer] = new float[STARTUP_DECLARATIONS.FactionCount - 4]
        {
            2000, 1000, 1000, 1000, 1000, 1000, 1000, 1000
        };
        ReputationMatrix[(int)FactionsEnum.B] = new float[STARTUP_DECLARATIONS.FactionCount - 5]
        {
            2000, 1000, 1000, 1000, 1000, 1000, 1000
        };
        ReputationMatrix[(int)FactionsEnum.Scavengers] = new float[STARTUP_DECLARATIONS.FactionCount - 6]
        {
            2000, 1000, 1000, 1000, 1000, 1000
        };
        ReputationMatrix[(int)FactionsEnum.Plantation] = new float[STARTUP_DECLARATIONS.FactionCount - 7]
        {
            2000, 1000, 1000, 1000, 1000
        };
        ReputationMatrix[(int)FactionsEnum.MidwayCityCivilian] = new float[STARTUP_DECLARATIONS.FactionCount - 8]
        {
            2000, 1000, 1000, 1000
        };
        ReputationMatrix[(int)FactionsEnum.FacelessReapers] = new float[STARTUP_DECLARATIONS.FactionCount - 9]
        {
            2000, 1000, 1000
        };
        ReputationMatrix[(int)FactionsEnum.Ascended] = new float[STARTUP_DECLARATIONS.FactionCount - 10]
        {
            2000, 1000
        };
        ReputationMatrix[(int)FactionsEnum.LightningCult] = new float[STARTUP_DECLARATIONS.FactionCount - 11]
        {
            2000
        };
    }

}
