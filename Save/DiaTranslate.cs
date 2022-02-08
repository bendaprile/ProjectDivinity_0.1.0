using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaTranslate : MonoBehaviour
{
    List<GameObject> diaFull = new List<GameObject>();
    public void DiaCompress(DiaRoot DR, out int start, out List<bool> enList)
    {
        DiaSerialize(DR.transform);

        start = -1;
        enList = new List<bool>();
        for (int i = 0; i < diaFull.Count; ++i)
        {
            enList.Add(diaFull[i].activeSelf);
            if (DR.ReturnStarting() == diaFull[i].transform)
            {
                start = i;
            }
        }
    }

    public void DiaDecompress(DiaRoot DR, int starting_in, List<bool> enList)
    {
        DiaSerialize(DR.transform);

        for (int i = 0; i < diaFull.Count; ++i)
        {
            diaFull[i].SetActive(enList[i]);
        }

        DR.ModifyStarting(diaFull[starting_in].transform); //This needs to be after so the merchant_quest check is after the SetActive
    }

    private void DiaSerialize(Transform trans)
    {
        DiaMaster DM = trans.GetComponent<DiaMaster>();
        if (DM) //Don't return because PlayerLines can be below NPCLines
        {
            diaFull.Add(trans.gameObject);
        }


        foreach (Transform child in trans)
        {
            DiaSerialize(child);
        }
    }
}
