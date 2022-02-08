using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InteractiveObject : InteractiveThing
{
    public SkillCheckDifficulty LockPickingRequirement = SkillCheckDifficulty._None_;
    public bool SetupOnceLogic = false; //Enables setup on object
    [SerializeField] protected ItemMaster keyToUnlock = null;

    protected PlayerStats playerStats;
    protected Inventory inventory;
    private World_Setup WS;




    public virtual void ChangeLockReq(SkillCheckDifficulty SCD)
    {
        LockPickingRequirement = SCD;
    }

    protected virtual void SuccessfullyActivated()
    {
        LockPickingRequirement = SkillCheckDifficulty._None_;
        QuestActiveLogic();
        if (SetupOnceLogic)
        {
            WS.Setup();
            SetupOnceLogic = false;
        }
    }



    protected override void Awake()
    {
        if(SetupOnceLogic)
        {
            WS = GetComponent<World_Setup>();
            Assert.IsNotNull(WS); //World Setup needs to be on this object for saving purposes
        }
        base.Awake();
        playerStats = player.GetComponent<PlayerStats>();
        inventory = FindObjectOfType<Inventory>();
    }
}
