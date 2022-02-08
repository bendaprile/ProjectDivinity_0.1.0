using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SendToEnemyOnEnable : MonoBehaviour
{
    [SerializeField] bool ReturnOnDisable = false;
    [SerializeField] bool DestroyOnDisable = false;

    private bool FirstStart = true;
    Transform NPCs;
    private List<(Transform, Vector3)> Children = new List<(Transform, Vector3)>();


    bool Activate = false;
    bool Deactivate = false;


    private void First_Start()
    {
        if (FirstStart)
        {
            Assert.IsFalse(ReturnOnDisable & DestroyOnDisable);
            FirstStart = false;
            NPCs = GameObject.Find("NPCs").transform;
        }
    }

    private void OnEnable()
    {
        First_Start();
        Activate = true;
        Deactivate = false;
    }

    private void OnDisable()
    {
        Deactivate = true;
        Activate = false;
    }



    private void FixedUpdate()
    {
        if (Activate)
        {
            Activate = false;
            for(int i = transform.childCount - 1; i >= 0; --i)
            {
                Transform child = transform.GetChild(i);
                Children.Add((child, child.position));
                FactionsEnum Faction = child.GetComponent<EnemyTemplateMaster>().Return_FactionEnum();
                child.SetParent(NPCs.Find(STARTUP_DECLARATIONS.FactionsEnumReverse[(int)Faction]));
            }
        }
        else if (Deactivate)
        {
            Deactivate = false;
            foreach ((Transform, Vector3) child in Children)
            {
                if (child.Item1)
                {
                    if (ReturnOnDisable)
                    {
                        child.Item1.SetParent(transform);
                        child.Item1.position = child.Item2;
                    }
                    else if (DestroyOnDisable)
                    {
                        Destroy(child.Item1.gameObject);
                    }
                }
            }
            Children.Clear();
        }
    }

}