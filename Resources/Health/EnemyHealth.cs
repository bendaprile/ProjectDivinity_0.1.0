using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private bool damageTextEnabled = false;
    [SerializeField] private GameObject damageTextPrefab = null;
    [SerializeField] private EnemyTemplateMaster ETmaster = null;

    private Transform FloatingTextParent;

    private GameObject regularDoT_number;
    private GameObject elementalDoT_number;
    private GameObject trueDoT_number;

    private EnemyHealthBar eHBar;

    protected override void Awake()
    {
        base.Awake();
        FloatingTextParent = GameObject.Find("FloatingTextParent").transform;
        regularDoT_number = null;
        elementalDoT_number = null;
        trueDoT_number = null;

        eHBar = transform.parent.GetComponentInChildren<EnemyHealthBar>();
        if (!eHBar)
        {
            Debug.Log(transform.parent.name);
        }
    }

    public override void take_damage(float damage, DamageSource DS, bool PlayerIsSource = false, bool knockback = false, Vector3 force = new Vector3(), float stun_duration = 0f, DamageType DT = DamageType.Regular, bool isDoT = false)
    {
        base.take_damage(damage, DS, PlayerIsSource, knockback, force, stun_duration, DT, isDoT);
        if (PlayerIsSource)
        {
            ETmaster.PlayerDamageTaken(ActualDamageDealt / maxHealth);
        }

        ETmaster.EnableAI(false); //TODO, playerdamage

        if (ETmaster && health > 0)
        {
            if (stun_duration > 0)
            {
                ETmaster.EnemyStun(stun_duration);
            }
            else if (knockback)
            {
                ETmaster.EnemyKnockback(force); //Dependent on the animation length
            }
        }
        else
        {
            ETmaster.Death();
        }

        if (damageTextEnabled)
        {
            DisplayDamageText(ActualDamageDealt, DT, isDoT);
        }
    }

    private void DisplayDamageText(float incomingDamage, DamageType DT, bool isDoT)
    {
        if (isDoT) //Keeps one number alive for every type of dot
        {
            if (DT == DamageType.Regular)
            {
                if (regularDoT_number == null)
                {
                    GameObject damageGameObject = Instantiate(damageTextPrefab, FloatingTextParent);
                    damageGameObject.GetComponent<FloatingTextScript>().Setup(transform.position, incomingDamage, DT);
                    regularDoT_number = damageGameObject;
                }
                else
                {
                    Vector3 offsetPOS = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    regularDoT_number.GetComponent<FloatingTextScript>().UpdateNumber(offsetPOS, incomingDamage);
                }
            }
        }
        else
        {
            GameObject damageGameObject = Instantiate(damageTextPrefab, FloatingTextParent);
            damageGameObject.GetComponent<FloatingTextScript>().Setup(transform.position, incomingDamage, DT);
        }
    }
}
