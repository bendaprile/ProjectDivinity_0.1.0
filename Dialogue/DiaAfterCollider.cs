using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DiaAfterCollider : MonoBehaviour
{
    [SerializeField] private Transform DiaRoot = null;
    [SerializeField] private bool DisableAfterUse = true;

    [SerializeField] private List<GameObject> Required_Flags = new List<GameObject>();
    [SerializeField] private List<bool> Required_Flags_Polarity = new List<bool>();

    [SerializeField] private bool Enter_true;

    private Collider this_collider;
    private UIController UIControl;

    private Zone_Flags ZF;

    private void Start()
    {
        this_collider = GetComponent<Collider>();
        UIControl = GameObject.Find("UI").GetComponent<UIController>();
        ZF = FindObjectOfType<Zone_Flags>();

        Assert.IsTrue(Required_Flags.Count == Required_Flags_Polarity.Count);
    }

    private void ModifyTrigger(bool state)
    {
        this_collider.enabled = state;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (Enter_true && collision.tag == "Player")
        {
            for(int i = 0; i < Required_Flags.Count; ++i)
            {
                if(ZF.CheckFlag(Required_Flags[i].name) != Required_Flags_Polarity[i])
                {
                    return;
                }
            }

            if (DisableAfterUse)
            {
                this_collider.enabled = false;
            }
            UIControl.DialogueMenuBool(DiaRoot);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!Enter_true && collision.tag == "Player")
        {
            for (int i = 0; i < Required_Flags.Count; ++i)
            {
                if (ZF.CheckFlag(Required_Flags[i].name) != Required_Flags_Polarity[i])
                {
                    return;
                }
            }

            if (DisableAfterUse)
            {
                this_collider.enabled = false;
            }
            UIControl.DialogueMenuBool(DiaRoot);
        }
    }
}
