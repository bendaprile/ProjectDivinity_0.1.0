using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AI_Master : MonoBehaviour
{
    Queue<GameObject> HighPriority = new Queue<GameObject>();
    Dictionary<GameObject, bool> in_HighPriority = new Dictionary<GameObject, bool>();

    Queue<GameObject> LowPriority = new Queue<GameObject>();
    Dictionary<GameObject, bool> in_LowPriority = new Dictionary<GameObject, bool>();

    [SerializeField] int LowPriority_per_frame = 5;
    [SerializeField] float HighPriority_per_frame = 10f;

    void Update()
    {
        for (int i = 0; i < LowPriority_per_frame; ++i)
        {
            if(LowPriority.Count == 0)
            {
                break;
            }
            GameObject temp = LowPriority.Dequeue();
            in_LowPriority.Remove(temp);

            if (temp && temp.activeInHierarchy)
            {
                temp.GetComponent<EnemyTemplateMaster>().AI_Master_Interface();
            }
        }

        for (int i = 0; i < HighPriority_per_frame; ++i)
        {
            if (HighPriority.Count == 0)
            {
                break;
            }
            GameObject temp = HighPriority.Dequeue();
            in_HighPriority.Remove(temp);

            if (temp && temp.activeInHierarchy)
            {
                temp.GetComponent<EnemyTemplateMaster>().AI_Master_Interface();
            }
        }
    }

    public void Request(GameObject Object, bool HiPri) 
    {
        if (HiPri)
        {
            //Debug.Log("HIrequest");
            if (!in_HighPriority.ContainsKey(Object))
            {
                in_HighPriority.Add(Object, true);
                HighPriority.Enqueue(Object);
            }
        }
        else
        {
            //Debug.Log("LWrequest");
            if (!in_LowPriority.ContainsKey(Object))
            {
                in_LowPriority.Add(Object, true);
                LowPriority.Enqueue(Object);
            }
        }
    }
}
