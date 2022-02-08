using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalItemStorage : MonoBehaviour
{
    InteractiveBox IB;
    HumanoidMaster HM;

    private void Awake()
    {
        IB = GetComponentInParent<InteractiveBox>();
        HM = GetComponentInParent<HumanoidMaster>();
    }


    public string ReturnName()
    {
        if (IB)
        {
            return IB.interactiveBoxName;
        }

        if (HM)
        {
            return HM.Return_Name();
        }

        return "";
    }

    public bool ReturnMerchant()
    {
        if (HM)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StoreItem(GameObject item)
    {
        item.transform.parent = transform;
    }

    public List<GameObject> ReturnItems()
    {
        List<GameObject> tempItems = new List<GameObject>();
        foreach (Transform item in transform)
        {
            tempItems.Add(item.gameObject);
        }
        return tempItems;
    }
}
