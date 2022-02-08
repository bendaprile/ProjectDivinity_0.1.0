using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Zone_Flags : MonoBehaviour
{
    [SerializeField] private Transform Master_Flags = null;
    protected Dictionary<string, bool> Flag = new Dictionary<string, bool>(); //This int is often used like a bool (0,1)
    protected QuestsHolder QH;
    private bool started = false;
    private Complex_Flag[] CF;


    public List<string> Return_Set_Flags()
    {
        Custom_Start();
        List<string> set_flags = new List<string>();
        foreach(string iter in Flag.Keys)
        {
            if (Flag[iter])
            {
                set_flags.Add(iter);
            }
        }
        return set_flags;
    }

    public bool CheckFlag(string FlagRef)
    {
        Custom_Start();
        return Flag[FlagRef];
    }

    public void SetFlag(GameObject FlagRef) //I am just using the GameObject for its unique number, I dont actually store anything to it
    {
        Custom_Start();
        Flag[FlagRef.name] = true;
        FlagEffects(FlagRef.name);
    }

    public void SetFlag_string(string FlagName)
    {
        Custom_Start();
        Flag[FlagName] = true;
        FlagEffects(FlagName);
    }



    private void Custom_Start() //This is needed because other functions call this when they awake and it is a race condition
    {
        if (started)
        {
            return;
        }

        started = true;
        QH = FindObjectOfType<QuestsHolder>();
        Recursive_Flag_Add(Master_Flags);

        for (int i = 5; i < STARTUP_DECLARATIONS.FactionCount; ++i)
        {
            Assert.IsTrue(Flag.ContainsKey(STARTUP_DECLARATIONS.FactionsEnumReverse[i] + "Hostile"), "There needs to be a hostile flag for the new faction: " + STARTUP_DECLARATIONS.FactionsEnumReverse[i]);
        }

        CF = GetComponentsInChildren<Complex_Flag>();
        foreach(Complex_Flag cf in CF)
        {
            cf.Setup();
        }
    }

    private void Recursive_Flag_Add(Transform PassThrough)
    {
        if (PassThrough.childCount == 0)
        {
            Flag.Add(PassThrough.name, false);
            return;
        }

        foreach(Transform child in PassThrough) 
        {
            Recursive_Flag_Add(child);
        }
    }

    private void FlagEffects(string FlagRef)
    {
        QH.CheckFlags();


        foreach (Complex_Flag cf in CF)
        {
            if (cf.gameObject.name != FlagRef && cf.LogicTest()) //Stop self recursive computation
            {
                SetFlag(cf.gameObject);
                Debug.Log(("Complex Flag Set", cf.gameObject));
            }
        }
    }
}
