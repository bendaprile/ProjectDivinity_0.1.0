using System.Collections;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private float SwapCD = 3f;
    [SerializeField] private float Equip_Unequip_time = 0.5f;
    [SerializeField] private GameObject MeleeWeapon;

    private Inventory Inventory;
    private Transform Storage;

    private GameObject currentWeaponGameObject = null;
    private GameObject itemParent;
    private PlayerAnimationUpdater pAU;
    private AbilitiesController AC;
    private Transform weaponDisplay;
    private bool lateStart = true;

    private bool weaponEquipped = false;
    private bool lastUsedWeapon;

    private float NextSwap = 0f;
    private float Current_Time_Limit = float.MaxValue; //Allows "Z" to be both quick and long press

    private void Start()
    {
        Inventory = GameObject.Find("Player").GetComponentInChildren<Inventory>();
        Storage = GameObject.Find("WeaponStorage").transform;
        pAU = GameObject.Find("Player").GetComponentInChildren<PlayerAnimationUpdater>();
        AC = FindObjectOfType<AbilitiesController>();
        weaponDisplay = GameObject.Find("WeaponDisplay").transform;
        lastUsedWeapon = false;
    }

    public void HandleWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Current_Time_Limit = Time.time + Equip_Unequip_time;
        }


        if (pAU.CanUpdateAnimation()) //Not rolling or melee
        {
            if(!AC.isAbilityInUse()) //Not casting
            {
                EquipUnequip();
                SwitchWeapon();

                if (weaponEquipped)
                {
                    Reload();
                    Attack();
                    EndAttack();
                }
            }
        }

        if (lateStart)
        {
            Transform temp = transform.FindDeepChild("RightHand");
            if(temp != null)
            {
                itemParent = temp.gameObject;
                UpdateHUDWeapon();
                lateStart = false;
            }
        }
    }

    public void RefreshWeapons()
    {
        if(currentWeaponGameObject != null)
        {
            currentWeaponGameObject.transform.parent = Storage;
            currentWeaponGameObject.gameObject.SetActive(false);
            currentWeaponGameObject = null;
        }
        UpdateHUDWeapon();
        weaponEquipped = false;
    }

    public void SetLastUsedWeapon(bool lastUsed)
    {
        lastUsedWeapon = false;
    }

    public void enableDisable(bool enable) //int so the animation controller can use it (called by ablities)
    {
        if (enable) //end sus attacks upon ability cast (which will disable the weapons)
        {
            StartCoroutine(DelayedReady());
        }
        else if (currentWeaponGameObject != null)
        {
            currentWeaponGameObject.GetComponent<Weapon>().EndSustainedAttack(true);
            currentWeaponGameObject.GetComponent<Weapon>().EndSustainedAttack2(true);
            RefreshWeapons();
        }
    }

    public void EndSustained()
    {
        if (currentWeaponGameObject != null)
        {
            currentWeaponGameObject.GetComponent<Weapon>().EndSustainedAttack(true);
            currentWeaponGameObject.GetComponent<Weapon>().EndSustainedAttack2(true);
        }
    }

    IEnumerator DelayedReady()
    {
        yield return new WaitForSeconds(.5f);
        ReadyWeapon();
    }

    // TODO: Handle input with input manager and not direct key references
    private void EquipUnequip()
    {
        if (!currentWeaponGameObject) //Equip a weapon
        {
            if (Time.time > Current_Time_Limit || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Current_Time_Limit = float.MaxValue;
                ReadyWeapon();
            }
        }
        else if (Time.time > Current_Time_Limit)
        {
            Current_Time_Limit = float.MaxValue;
            RefreshWeapons();
        }
    }

    private void ReadyWeapon()
    {
        bool AnyWeaponUsable = false;
        if (Inventory.ReturnWeapon(lastUsedWeapon ? 1 : 0)) //Normal
        {
            AnyWeaponUsable = true;
        }
        else if (Inventory.ReturnWeapon(lastUsedWeapon ? 0 : 1)) //Inverted
        {
            AnyWeaponUsable = true;
            lastUsedWeapon = !lastUsedWeapon;
        }

        if (AnyWeaponUsable)
        {
            currentWeaponGameObject = Inventory.ReturnWeapon(lastUsedWeapon ? 1 : 0); //Normal
            weaponEquipped = true;
            CleanParenting();
        }
        UpdateHUDWeapon();
    }


    // TODO: Handle input with input manager and not direct key references
    public void SwitchWeapon(bool force = false)
    {
        if((Current_Time_Limit != float.MaxValue && Input.GetKeyUp(KeyCode.Z) || force))
        {
            Current_Time_Limit = float.MaxValue;

            if ((Time.time >= NextSwap && weaponEquipped) || weaponEquipped && force)
            {

                if (Inventory.ReturnWeapon(lastUsedWeapon ? 0 : 1) != null) //Inverted
                {
                    NextSwap = Time.time + SwapCD;
                    currentWeaponGameObject.transform.parent = Storage;
                    currentWeaponGameObject.gameObject.SetActive(false);
                    lastUsedWeapon = !lastUsedWeapon;
                    currentWeaponGameObject = Inventory.ReturnWeapon(lastUsedWeapon ? 1 : 0); //Normal
                    CleanParenting();
                }
                UpdateHUDWeapon();
            }
        }
    }

    // TODO: Handle input with input manager and not direct key references
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentWeaponGameObject.GetComponent<Weapon>().Attack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            currentWeaponGameObject.GetComponent<Weapon>().Attack2();
        }
    }

    private void EndAttack() //force for ability or roll
    {
        if (Input.GetMouseButtonUp(0))
        {
            currentWeaponGameObject.GetComponent<Weapon>().EndSustainedAttack();
        }

        if (Input.GetMouseButtonUp(1))
        {
            currentWeaponGameObject.GetComponent<Weapon>().EndSustainedAttack2();
        }
    }

    public void Reload(bool force = false)
    {
        if(currentWeaponGameObject && currentWeaponGameObject.GetComponent<Weapon>().ReturnWeaponType() == WeaponType.Ranged)
        {
            if (force)
            {
                currentWeaponGameObject.GetComponent<Ranged>().AttemptReload(force);
            }
            else if (Input.GetKeyDown("r"))
            {
                currentWeaponGameObject.GetComponent<Ranged>().AttemptReload();
            } 
        }
    }

    public void AnimationTriggerAttack()
    {
        if (currentWeaponGameObject && currentWeaponGameObject.activeSelf) //Second check here is to make sure the weapon is active
        {
            currentWeaponGameObject.GetComponent<Weapon>().AnimationTriggerAttack();
        }
    }

    private void CleanParenting() //Takes the Vector3 values and makes them into local values. This way Items can be placed precisely
    {
        currentWeaponGameObject.SetActive(true);
        currentWeaponGameObject.transform.SetParent(itemParent.transform);
        Weapon tempWeapon = currentWeaponGameObject.GetComponent<Weapon>();

        currentWeaponGameObject.transform.localEulerAngles = tempWeapon.StartingRotation;
        currentWeaponGameObject.transform.localPosition = tempWeapon.StartingLocation;
        currentWeaponGameObject.transform.localScale = tempWeapon.StartingScale;
    }

    private void UpdateHUDWeapon()
    {
        if (currentWeaponGameObject)
        {
            weaponDisplay.Find("Image").GetComponent<Image>().sprite = currentWeaponGameObject.GetComponent<Weapon>().item_sprite;
            weaponDisplay.Find("Image").GetComponent<Image>().color = Color.white;
            weaponDisplay.Find("Border").GetComponent<Image>().color
                = STARTUP_DECLARATIONS.itemQualityColors[currentWeaponGameObject.GetComponent<Weapon>().ReturnBasicStats().Item5];
        }
        else
        {
            weaponDisplay.Find("Image").GetComponent<Image>().sprite = null;
            weaponDisplay.Find("Image").GetComponent<Image>().color = Color.clear;
            weaponDisplay.Find("Border").GetComponent<Image>().color = new Color32(130, 90, 36, 255);
        }
    }

    public GameObject getCurrentWeapon()
    {
        return currentWeaponGameObject;
    }
}
