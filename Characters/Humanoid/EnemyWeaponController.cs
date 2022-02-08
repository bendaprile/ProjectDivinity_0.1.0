using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : EnemyAbility //Just so that I can use clean_LoS
{
    [SerializeField] GameObject WeaponSlot = null;

    private HumanoidMaster HM;
    private GameObject currentWeaponGameObject;
    private GameObject itemParent;
    private HumanoidMeleeBox HMB;

    private bool lateStart = true;

    protected override void Start()
    {
        base.Start();
        HM = GetComponentInParent<HumanoidMaster>();
        HMB = HM.GetComponentInChildren<HumanoidMeleeBox>();
    }

    public void UpdateWeaponSlot(GameObject Weapon_in)
    {
        WeaponSlot = Weapon_in;
    }

    public HumanoidLogicMode Attack()
    {
        if (lateStart)
        {
            Transform temp = transform.FindDeepChild("RightHand");
            if (temp != null) //Waits for body to be made
            {
                itemParent = temp.gameObject;
                lateStart = false;

                if (WeaponSlot)
                {
                    currentWeaponGameObject = Instantiate(WeaponSlot);
                    CleanParentingEnemy();
                    currentWeaponGameObject.GetComponent<Weapon>().StartWeapon(true, GetComponentInParent<EnemyTemplateMaster>(), HM.Return_WeaponExpertise());
                }
            }
        }


        HumanoidLogicMode tempMode;
        if (currentWeaponGameObject != null)
        {
            WeaponType weaponType = currentWeaponGameObject.GetComponent<Weapon>().ReturnWeaponType();

            if (weaponType == WeaponType.Melee_1H || weaponType == WeaponType.Melee_2H) //Melee
            {
                tempMode = HumanoidLogicMode.Charge_in;

                if (HMB.TargetsInMeleeRange.Count > 0)
                {
                    currentWeaponGameObject.GetComponent<Weapon>().EnemyAttack();
                }
            }
            else //Ranged
            {
                if (clean_LoS(false))
                {
                    currentWeaponGameObject.GetComponent<Weapon>().EnemyAttack();
                    tempMode = HumanoidLogicMode.Kiting;
                }
                else
                {
                    tempMode = HumanoidLogicMode.Kiting_reposition;
                }
            }
        }
        else
        {
            tempMode = HumanoidLogicMode.Flee;
        }

        return tempMode;
    }

    public GameObject ReturnWeapon()
    {
        return currentWeaponGameObject;
    }

    public void AnimationTriggerAttack()
    {
        currentWeaponGameObject.GetComponent<Weapon>().AnimationTriggerAttack();
    }

    private void CleanParentingEnemy() //Takes the global transform values and makes them into local values. This way Items can be placed precisely
    {
        currentWeaponGameObject.SetActive(true);
        currentWeaponGameObject.transform.SetParent(itemParent.transform);
        Weapon tempWeapon = currentWeaponGameObject.GetComponent<Weapon>();

        currentWeaponGameObject.transform.localEulerAngles = tempWeapon.StartingRotation;
        currentWeaponGameObject.transform.localPosition = tempWeapon.StartingLocation;
        currentWeaponGameObject.transform.localScale = tempWeapon.StartingScale;
    }

    public List<Transform> ReturnTiMR()
    {
        for (int i = HMB.TargetsInMeleeRange.Count - 1; i >= 0; --i)
        {
            if (!HMB.TargetsInMeleeRange[i] || HMB.TargetsInMeleeRange[i].tag == "DeadEnemy")
            {
                HMB.TargetsInMeleeRange.RemoveAt(i);
            }
        }
        return HMB.TargetsInMeleeRange;
    }


}
