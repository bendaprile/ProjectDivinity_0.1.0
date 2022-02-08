using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoMaster : MonoBehaviour
{
    [SerializeField] private float hitOffset = 0f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 0, 0);
    [SerializeField] protected GameObject HitEffect;

    [SerializeField] protected GameObject DetachedAudio;
    [SerializeField] protected AudioClip TargetHitSound;
    [SerializeField] protected AudioClip OtherHitSound;


    protected bool destroyOnEnemy;
    protected int damage;
    protected DamageType dt;
    protected DamageSource DS;
    public bool Player_Fired = true;
    protected CustomReputation CustomRep = CustomReputation.Standard;

    protected FactionsEnum FacEnum;
    protected FactionLogic FL;

    protected bool disabled = false; //using this because destroyImmediate is bad practie

    public virtual void Setup(int dam, DamageSource DS_in, bool destEnem = true, DamageType dt_in = DamageType.Regular) //NEED to destroy the parent on_disable for special bullets that have their main logic on a child (None yet)
    {
        damage = dam;
        DS = DS_in;
        destroyOnEnemy = destEnem;
        dt = dt_in;
        Destroy(gameObject, 5f);
    }

    protected virtual void OnDisable()
    {
        if (HitEffect != null)
        {
            var hitInstance = Instantiate(HitEffect, transform.parent.parent.Find("NonLogicProjectiles"));
            hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0);
            hitInstance.transform.position = transform.position + transform.forward * -hitOffset;
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
    }

    public void IndependentAudio(AudioClip audio_in)
    {
        if (!audio_in)
        {
            return;
        }
        var audioInstance = Instantiate(DetachedAudio, transform.parent.parent.Find("NonLogicProjectiles"));
        audioInstance.transform.position = transform.position;
        audioInstance.GetComponent<AudioSource>().PlayOneShot(audio_in);
        Destroy(audioInstance, audio_in.length);
    }

    public virtual void AdditionalNPCSetup(FactionsEnum Fenum_in, FactionLogic FL_in, CustomReputation Custom_Rep_in)
    {
        FacEnum = Fenum_in;
        FL = FL_in;
        CustomRep = Custom_Rep_in;
        Player_Fired = false;
    }

    public void KineticReversalHelper()
    {
        Player_Fired = true;
        GetComponent<Rigidbody>().velocity = -GetComponent<Rigidbody>().velocity;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!disabled) //Prevents projectile from damaging more than 1 object
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Terrain") || other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                Destroy(gameObject);
                IndependentAudio(OtherHitSound);
            }

            if (Player_Fired)
            {
                if (other.tag == "BasicEnemy")
                {
                    DealDamage(other, true);
                    IndependentAudio(TargetHitSound);
                }
            }
            else
            {
                if (other.tag == "Player" && FL.ReturnIsEnemy(FacEnum, FactionsEnum.Player, CustomRep))
                {
                    DealDamage(other, false);
                    IndependentAudio(TargetHitSound);
                }

                if (other.tag == "BasicEnemy" && FL.ReturnIsEnemy(FacEnum, other.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), CustomRep))
                {
                    DealDamage(other, false);
                    IndependentAudio(TargetHitSound);
                }
            }
        }
    }

    public (float, DamageSource, DamageType) returnDamageStats()
    {
        return (damage, DS, dt);
    }

    protected virtual void DealDamage(Collider other, bool PlayerCast)
    {
        other.gameObject.GetComponent<Health>().take_damage(damage, DS, PlayerCast, DT: dt);
        if (destroyOnEnemy)
        {
            disabled = true;
            Destroy(gameObject);
        }
    }
}
