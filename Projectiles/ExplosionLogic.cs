using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLogic : MonoBehaviour
{
    private int damage;
    private DamageSource DS;
    private DamageType dt;
    private bool Player_Fired = true;
    protected CustomReputation CustomRep = CustomReputation.Standard;

    protected FactionsEnum FacEnum;
    protected FactionLogic FL;
    private List<Collider> SeenColliders = new List<Collider>();

    public void Setup(int dam, DamageSource DS_in, DamageType dt_in, float DestroyTime = 2f)
    {
        damage = dam;
        DS = DS_in;
        dt = dt_in;
        Destroy(gameObject, DestroyTime);
    }

    public virtual void AdditionalNPCSetup(FactionsEnum Fenum_in, FactionLogic FL_in, CustomReputation CustomRep_in)
    {
        FacEnum = Fenum_in;
        FL = FL_in;
        CustomRep = CustomRep_in;
        Player_Fired = false;
    }

    protected virtual void DealDamage(Collider other, bool PlayerCast)
    {
        other.gameObject.GetComponent<Health>().take_damage(damage, DS, PlayerCast, DT: dt);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(SeenColliders.Contains(other))
        {
            return;
        }
        else
        {
            SeenColliders.Add(other);
        }


        if (Player_Fired)
        {
            if (other.tag == "BasicEnemy")
            {
                DealDamage(other, true);
            }
        }
        else
        {
            if (other.tag == "Player" && FL.ReturnIsEnemy(FacEnum, FactionsEnum.Player, CustomRep))
            {
                DealDamage(other, false);
            }

            if (other.tag == "BasicEnemy" && FL.ReturnIsEnemy(FacEnum, other.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), CustomRep))
            {
                DealDamage(other, false);
            }
        }
    }
}
