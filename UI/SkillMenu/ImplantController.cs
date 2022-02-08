using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImplantController : MonoBehaviour
{
    [SerializeField] private GameObject Cell = null;
    public AptitudeEnum Type; //DO NOT WRITE TO

    private Transform Grid;
    private Transform Implants;
    private List<(GameObject, Vector3)> ImplantsBackupLocations = new List<(GameObject, Vector3)>();
    private Transform TempRemoveStorage;
    private List<GameObject> TempAddReference = new List<GameObject>();

    private TextMeshProUGUI Valid;
    private TextMeshProUGUI implantPoints;

    private Inventory inventory;
    private PlayerStats playerStats;
    private ActivePerks activePerks;
    private ImplantUIHolder iUIh;
    private ImplantCellPrefab[,] cellArray = new ImplantCellPrefab[16, 16]; //bottom-left to top-right

    private bool first_start = true;

    public void AddReference(GameObject input)
    {
        TempAddReference.Add(input);
    }

    public List<GameObject> return_implants()
    {
        List<GameObject> impList = new List<GameObject>();
        foreach((GameObject, Vector3) imp in ImplantsBackupLocations)
        {
            impList.Add(imp.Item1);
        }
        return impList;
    }

    public void first_start_func()
    {
        if (first_start)
        {
            playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
            activePerks = GameObject.Find("Player").GetComponentInChildren<ActivePerks>();
            Implants = transform.Find("Implants");
            Grid = transform.Find("Grid");
            Valid = transform.Find("ValidText").GetComponent<TextMeshProUGUI>();
            implantPoints = transform.Find("ImplantPoints").GetComponent<TextMeshProUGUI>();
            TempRemoveStorage = transform.parent.Find("ImplantTempRemoveStorage"); //Make sure this can be accessed when disabled for loading
            inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            iUIh = FindObjectOfType<ImplantUIHolder>(true);

            first_start = false;
            bool locked;
            for(int j = 0; j < 16; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    GameObject temp = Instantiate(Cell, Grid);

                    if(i >= 6 && i <= 9 && j >= 6 && j <= 9)
                    {
                        locked = false;
                    }
                    else
                    {
                        locked = true;
                    }
                    cellArray[i, j] = temp.GetComponent<ImplantCellPrefab>();


                    int cost = Mathf.Max((int)Mathf.Abs(7.5f - i), (int)Mathf.Abs(7.5f - j));
                    cost -= 1;

                    cellArray[i, j].Setup(locked, cost, this);
                }
            }
        }
    }

    private void OnEnable()
    {
        first_start_func();
    }

    private void Update()
    {
        implantPoints.text = "POINTS: " + playerStats.returnImplantPoints(Type).ToString();
        Valid.text = "VALID";
    }

    public bool AttemptTempCellChange(int cost, bool unlock)
    {
        if (unlock)
        {
            if(cost <= playerStats.returnImplantPoints(Type))
            {
                playerStats.modifyImplantPoints(Type, - cost);
                implantPoints.text = playerStats.returnImplantPoints(Type).ToString();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            playerStats.modifyImplantPoints(Type, cost);
            implantPoints.text = playerStats.returnImplantPoints(Type).ToString();
            return true;
        }
    }
     
    public void FinalizeImplants(bool external_load = false)
    {
        for (int j = 0; j < 16; j++)
        {
            for (int i = 0; i < 16; i++)
            {
                cellArray[i, j].setUsed(false);
            }
        }

        bool passed = true;
        foreach (Transform child in Implants) //(Implants * 256) Fine... I guess...
        {
            ImplantPrefab implantScript = child.GetComponent<ImplantPrefab>();
            for (int j = 0; j < 16; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (implantScript.ReturnCellUsed(i, j))
                    {
                        if (cellArray[i, j].returnLocked() || cellArray[i, j].returnUsed())
                        {
                            passed = false;
                        }
                        else
                        {
                            cellArray[i, j].setUsed(true);
                        }
                    }
                }
            }
        }

        if (passed)
        {
            HandleEffects(false); //remove effects
            ImplantsBackupLocations.Clear();

            foreach (Transform child in Implants)
            {
                Debug.Log(child.GetComponent<RectTransform>().position);
                ImplantsBackupLocations.Add((child.gameObject, child.GetComponent<RectTransform>().position));
            }

            HandleEffects(true); //add effects
            Valid.text = "VALID";
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    cellArray[i, j].LockInTemp();
                }
            }

            for(int i = TempRemoveStorage.childCount - 1; i >= 0; --i) //Send items to inventory
            {
                inventory.AddItem(TempRemoveStorage.GetChild(i).gameObject);
            }
            TempAddReference.Clear();

            if (!external_load)
            {
                iUIh.Refresh();
            }
        }
        else
        {
            Valid.text = "INVALID";
        }
    }

    private void HandleEffects(bool Add) // O(n^2) (Could make a dict) TODO
    {
        if (!Add)
        {
            activePerks.ResetImplantPerks();
        }

        for (int i = 0; i < ImplantsBackupLocations.Count; i++) //((n+1)(n)/2) ... n <= 256
        {
            ImplantStats temp = ImplantsBackupLocations[i].Item1.GetComponent<ImplantStats>();

            int count = 0;
            string name = temp.ReturnBasicStats().Item3;

            for (int j = 0; j < i; j++) //Name duplicates
            {
                if (ImplantsBackupLocations[i].Item1.GetComponent<ImplantStats>().ReturnBasicStats().Item3 == name)
                {
                    count += 1;
                }
            }

            if (count > 0)
            {
                name += " " + (count + 1).ToString();
            }

            if (Add)
            {
                if (temp.AttributeModifier)
                {
                    playerStats.AddAttributeEffect(temp.attributeName, name, temp.isAdd, temp.value);
                }

                if (temp.ReturnEAP().Item1)
                {
                    activePerks.AddImplantPerk(temp.ReturnEAP().Item2, temp.ReturnEAP().Item3);
                }
            }
            else
            {
                if (temp.AttributeModifier)
                {
                    playerStats.RemoveAttributeEffect(temp.attributeName, name, temp.isAdd);
                }
            }

        }
    }

    private void OnDisable()
    {
        foreach (GameObject implant in TempAddReference) //Must be before TempRemovalStorage so that just added items are not destroyed. This will transfer the parent so its fine.
        {
            if (inventory) //Don't do if game is ending
            {
                inventory.AddItem(implant); //Note there is a case where you add an implant then remove it. It will be in the Temp storage. These refs can still bring it back to the inv
            }
        }
        TempAddReference.Clear();

        foreach ((GameObject, Vector3) child in ImplantsBackupLocations)
        {
            child.Item1.SetActive(true);
            child.Item1.transform.parent = Implants;
            child.Item1.GetComponent<RectTransform>().position = child.Item2; //(will be in implants or in TempRemoveStorage)
        }
    }
}
