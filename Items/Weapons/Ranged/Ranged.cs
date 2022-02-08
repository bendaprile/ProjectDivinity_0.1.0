using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Ranged : Weapon
{
    public RangedAnimation rangedAnimation;
    public AudioClip FiringSound;
    public AudioClip ReloadSound;
    public AudioClip OutOfAmmoSound;
    public GameObject FlashEffect;

    [SerializeField] protected int ammoCapacity;
    [SerializeField] protected int damage;
    [SerializeField] protected float energyCost;
    [SerializeField] protected float reloadEnergyCost;
    [SerializeField] protected float projectileVelocity;

    [SerializeField] protected Transform projectile = null;
    [SerializeField] protected Transform projectileSpawnPoint = null;

    private int currentAmmoCount;
    protected bool attacking;

    protected AmmoDisplay ammoDisplay;
    protected Transform projectileParent;
    protected AudioSource audioSource;

    private bool Started = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void AttemptReload(bool force = false)
    {
        Assert.IsFalse(EnemyWeapon);
        if (force)
        {
            Reload(false);
        }
        else if (currentAmmoCount != ammoCapacity)
        {
            if (energy.Drain_ES(true, reloadEnergyCost))
            {
                Reload(true);
            }
        }
    }

    private void Reload(bool anim)
    {
        if (anim)
        {
            animationUpdater.ReloadAnimation();
        }
        audioSource.PlayOneShot(ReloadSound);
        currentAmmoCount = ammoCapacity;
        ammoDisplay.Reload();
    }

    public bool isMagazineEmpty()
    {
        return currentAmmoCount == 0;
    }

    ///////////////////////////////////////////////////////////////////////

    public Ranged()
    {
        weaponType = WeaponType.Ranged;
    }

    public override void StartWeapon(bool EnemyWeapon_in = false, EnemyTemplateMaster ETM_in = null, HumanoidWeaponExpertise humanoidWeaponExpertise = HumanoidWeaponExpertise.Adept)
    {
        base.StartWeapon(EnemyWeapon_in, ETM_in, humanoidWeaponExpertise);
        EnemyWeapon = EnemyWeapon_in;
        Started = true;
        if (EnemyWeapon_in)
        {
            projectileParent = GameObject.Find("EnemyProjectiles").transform;
        }
        else
        {
            ammoDisplay = GameObject.Find("AmmoDisplay").GetComponent<AmmoDisplay>();
            currentAmmoCount = ammoCapacity;
            projectileParent = GameObject.Find("PlayerProjectiles").transform;
        }
    }

    private void OnEnable()
    {
        if (Started & !EnemyWeapon)
        {
            ammoDisplay.Setup(ammoCapacity, currentAmmoCount);
        }
    }

    private void OnDisable()
    {
        if (Started && ammoDisplay)
        {
            ammoDisplay.Setup(0, 0);
        }
    }

    protected int ReturnFinalDamage(float input)
    {
        if (EnemyWeapon)
        {
            return (int)(input);
        }
        else
        {
            return (int)(stats.ReturnDamageMult(input, DS));
        }
    }

    protected Transform fireProjectile(bool first_call, float angle_in, bool use_ammo = true)
    {
        if (first_call)
        {
            audioSource.PlayOneShot(FiringSound);

            if (FlashEffect != null)
            {
                var flashInstance = Instantiate(FlashEffect, projectileSpawnPoint.position, Quaternion.identity, transform);
                flashInstance.transform.forward = gameObject.transform.forward;
                var flashPs = flashInstance.GetComponent<ParticleSystem>();
                if (flashPs != null)
                {
                    Destroy(flashInstance, flashPs.main.duration);
                }
                else
                {
                    var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(flashInstance, flashPsParts.main.duration);
                }
            }
        }

        if (use_ammo && !EnemyWeapon)
        {
            UseAmmoFunc();
        }

        Transform tempTrans = Instantiate(projectile, projectileParent);
        tempTrans.position = projectileSpawnPoint.position;
        tempTrans.eulerAngles = new Vector3(0f, (angle_in * Mathf.Rad2Deg), 0f);
        tempTrans.GetComponentInChildren<Rigidbody>().velocity = new Vector3(Mathf.Sin(angle_in) * projectileVelocity, 0f, Mathf.Cos(angle_in) * projectileVelocity);

        return tempTrans;
    }

    protected float CustomAngle()
    {
        float angle;

        if (EnemyWeapon)
        {
            Vector3 pos = ETM.Return_Current_Target().position;
            if(WeaponExpertise == HumanoidWeaponExpertise.Commando) //Predict Movement
            {
                Vector3 vel = ETM.Return_Current_Target().GetComponentInParent<Rigidbody>().velocity;
                float time_est = (pos - transform.position).magnitude / projectileVelocity;
                pos += vel * time_est;
            }
            pos -= transform.position;

            angle = Mathf.Atan2(pos.x, pos.z);
        }
        else
        {
            Vector3 mousePos3d = new Vector3();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitray;
            int layerMask = (LayerMask.GetMask("Terrain"));
            if (Physics.Raycast(ray, out hitray, Mathf.Infinity, layerMask))
            {
                mousePos3d = hitray.point;
            }

            float special_offset = -Mathf.Tan(Mathf.PI / 2 - (Camera.main.transform.rotation.eulerAngles.x * Mathf.Deg2Rad)) * 2; //Made to hit 2 meter tall enemies. //negitive so it is the opposite angle
            angle = Mathf.Atan2((mousePos3d.x + special_offset * Mathf.Sin(Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) - transform.position.x, (mousePos3d.z + special_offset * Mathf.Cos(Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) - transform.position.z);
        }

        return angle;
    }

    protected void UseAmmoFunc(int amount = 1)
    {
        Assert.IsFalse(EnemyWeapon);
        currentAmmoCount -= amount;
        ammoDisplay.UseAmmo();
    }

    public bool CheckAmmo(int amount = 1)
    {
        if(currentAmmoCount >= amount)
        {
            return true;
        }
        else
        {
            audioSource.PlayOneShot(OutOfAmmoSound);
            return false;
        }
    }

    protected override void AdvStatsHelper(List<(string, string)> tempList)
    {
        base.AdvStatsHelper(tempList);
        tempList.Add(("Ammo Capacity:", ammoCapacity.ToString()));
        tempList.Add(("Reload Energy Cost:", reloadEnergyCost.ToString()));
        tempList.Add(("Damage:", damage.ToString()));
        tempList.Add(("Firing Energy Cost:", energyCost.ToString()));
    }
}
