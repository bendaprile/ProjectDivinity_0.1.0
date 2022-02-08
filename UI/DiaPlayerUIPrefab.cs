using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaPlayerUIPrefab : MonoBehaviour
{
    DiaParent diaParent;
    bool pressable;

    DiaPlayerLine TempChildStorage;



    public void Setup(DiaPlayerLine TempChild)
    {
        TempChildStorage = TempChild;
        TextMeshProUGUI textRef = transform.GetComponent<TextMeshProUGUI>();
        pressable = TempChild.Check_Accessible(textRef);

        diaParent = GameObject.Find("DialogueMenu").GetComponent<DiaParent>();
    }


    public void ButtonPressed()
    {
        if (pressable)
        {
            if(TempChildStorage.return_new_start() != null)
            {
                TempChildStorage.GetComponentInParent<DiaRoot>().ModifyStarting(TempChildStorage.return_new_start());
            }

            TempChildStorage.GameplayLogic(false);

            diaParent.Continue(TempChildStorage.return_dest());

            if (TempChildStorage.Merchant) //Must be after diaParent.Continue so that the UI is disabled first
            {
                UIController UIControl = FindObjectOfType<UIController>();
                UIControl.OpenInteractiveMenu(TempChildStorage.Merchant);
            }
        }
    }

}
