using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 0;
    [SerializeField] protected float healthRegen = 0;
    [SerializeField] protected int MaxPlating = 0;
    [SerializeField] protected int MaxShield = 0;
    [SerializeField] protected int Armor = 0;

    [SerializeField] protected GameObject[] BloodArray;
    [SerializeField] protected Image healthSlider = null;
    [SerializeField] protected Image platingSlider = null;
    [SerializeField] protected Image shieldSlider = null;

    private int BloodIter;
    protected Transform NonLogicProjectiles;


    protected float health;
    protected float Plating;
    protected float Shield;



    private float ModifiedDamage; //here so override functions can use it
    protected float ActualDamageDealt;


    private string original_tag;

    public void set_is_immortal(bool set)
    {
        if (set)
        {
            gameObject.tag = "Untagged";
        }
        else
        {
            gameObject.tag = original_tag;
        }

    }

    protected virtual void Awake()
    {
        original_tag = gameObject.tag;
        BloodIter = Random.Range(0, BloodArray.Length);
        NonLogicProjectiles = GameObject.Find("NonLogicProjectiles").transform;
        health = maxHealth;
        Plating = MaxPlating;
        Shield = MaxShield;

        if (healthSlider)
        {
            UpdateHealthBar();
        }
    }

    public void modify_maxHealth(float value)
    {
        maxHealth = value;
        health += value;

        if (healthSlider)
        {
            UpdateHealthBar();
        }
    }

    public float ReturnMaxHealth()
    {
        return maxHealth;
    }

    public void modify_healthRegen(float value)
    {
        healthRegen = value;
    }

    public void modify_Plating(int value)
    {
        MaxPlating = value;
        Plating = value;
    }

    public void modify_Shield(int value)
    {
        MaxShield = value;
        Shield = value;
    }

    public void modify_Armor(int value)
    {
        Armor = value;
    }

    public virtual void take_damage(float damage, DamageSource DS, bool PlayerIsSource = false, bool knockback = false, Vector3 force = new Vector3(), float stun_duration = 0f, DamageType DT = DamageType.Regular, bool isDoT = false)
    {
        ModifiedDamage = HealthCalculation(damage, DT);
        ActualDamageDealt = 0;

        if (Shield > 0 && ModifiedDamage > 0)
        {
            if(ModifiedDamage * ShieldCalc(DS) <= Shield)
            {
                Shield -= ModifiedDamage * ShieldCalc(DS);
                ActualDamageDealt += ModifiedDamage * ShieldCalc(DS);
                ModifiedDamage = 0;
            }
            else
            {
                ActualDamageDealt += Shield;
                ModifiedDamage -= Shield / ShieldCalc(DS);
                Shield = 0;
            }
        }  
        
        if(Plating > 0 && ModifiedDamage > 0) //Can take damage after shield if shield goes to 0
        {
            if (ModifiedDamage * PlatingCalc(DS) <= Plating)
            {
                Plating -= ModifiedDamage * PlatingCalc(DS);
                ActualDamageDealt += ModifiedDamage * PlatingCalc(DS);
                ModifiedDamage = 0;
            }
            else
            {
                ModifiedDamage -= Plating / PlatingCalc(DS);
                ActualDamageDealt += Plating;
                Plating = 0;
            }
        }

        if(ModifiedDamage > 0) //Can take damage after plating if plating goes to 0
        {
            health -= ModifiedDamage;
            ActualDamageDealt += ModifiedDamage;
            if (!isDoT && BloodArray.Length > 0)
            {
                GameObject Bloodtemp = Instantiate(BloodArray[BloodIter], NonLogicProjectiles);
                Bloodtemp.transform.position = transform.position;
                Bloodtemp.transform.rotation = transform.rotation;
                Destroy(Bloodtemp, 20f);
                BloodIter = (BloodIter + 1) % BloodArray.Length;
            }
        }


        if (healthSlider)
        {
            UpdateHealthBar();
        }
    }

    public float ReturnCurrentHealth()
    {
        return health;
    }

    public float ReturnCurrentHealthProportion()
    {
        return health / maxHealth;
    }

    public void heal(float amount)
    {
        if(health < maxHealth)
        {
            health += amount;
        }

        if (healthSlider)
        {
            UpdateHealthBar();
        }
    }

    protected float HealthCalculation(float damage, DamageType DT)
    {
        if (DT == DamageType.Regular)
        {
            float resist = (100f / (Armor + 100f));
            damage *= resist;
        }

        if(damage < 0)
        {
            damage = 0;
        }

        return damage;
    }

   protected void UpdateHealthBar()
    {
        healthSlider.fillAmount = health / maxHealth;

        if (platingSlider)
        {
            if(MaxPlating == 0)
            {
                platingSlider.fillAmount = 0;
            }
            else
            {
                platingSlider.fillAmount = Plating / MaxPlating;
            }
        }

        if (shieldSlider)
        {
            if (MaxShield == 0)
            {
                shieldSlider.fillAmount = 0;
            }
            else
            {
                shieldSlider.fillAmount = Shield / MaxShield;
            }
        }
    }

   protected virtual void FixedUpdate()
    {
        heal(healthRegen * Time.fixedDeltaTime);

        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    protected float ShieldCalc(DamageSource DS)
    {
        if(DS == DamageSource.VigorBased)
        {
            return 0.5f;
        }
        else if (DS == DamageSource.CerebralBased)
        {
            return 0.75f;
        }
        else //FinesseBased
        {
            return 1f;
        }
    }

    protected float PlatingCalc(DamageSource DS)
    {
        if (DS == DamageSource.VigorBased)
        {
            return 1f;
        }
        else if (DS == DamageSource.CerebralBased)
        {
            return 0.75f;
        }
        else //FinesseBased
        {
            return 0.5f;
        }
    }
}
