using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplantUIHolder : MonoBehaviour
{
    [SerializeField] GameObject ImplantUIPrefab = null;
    private Inventory InventoryScript = null;
    private Transform tempStorage = null;

    private bool firstStart = true;

    AptitudeEnum CurrentSelected;

    void FirstStart()
    {
        InventoryScript = GameObject.Find("Player").GetComponentInChildren<Inventory>();
        tempStorage = GameObject.Find("ImplantTempRemoveStorage").transform;
        firstStart = false;
    }

    private void OnEnable()
    {
        if (firstStart)
        {
            FirstStart();
        }
        Refresh();
    }

    public void ChangeType(AptitudeEnum AE)
    {
        CurrentSelected = AE;
        Refresh();
    }

    public void Refresh()
    {
        foreach(Transform child in transform) //Destroys after frame
        {
            Destroy(child.gameObject);
        }

        List<GameObject> items = InventoryScript.ReturnItems(ItemTypeEnum.Implant);

        foreach (GameObject item in items)
        {
            if(item.GetComponent<ImplantStats>().AptitudeType == CurrentSelected)
            {
                GameObject UIFab = Instantiate(ImplantUIPrefab, transform);
                UIFab.GetComponent<ImplantUIPrefab>().Setup(true, item, GetComponent<ImplantUIHolder>(), InventoryScript);
            }
        }

        foreach (Transform item in tempStorage)
        {
            if (item.GetComponent<ImplantStats>().AptitudeType == CurrentSelected)
            {
                GameObject UIFab = Instantiate(ImplantUIPrefab, transform);
                UIFab.GetComponent<ImplantUIPrefab>().Setup(false, item.gameObject, GetComponent<ImplantUIHolder>(), InventoryScript);
            }
        }
    }
}
