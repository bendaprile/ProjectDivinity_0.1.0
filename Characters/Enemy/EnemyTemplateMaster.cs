using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

public class EnemyTemplateMaster : MonoBehaviour
{
    public float DifficultyPoints = 1;
    public float SpawnPadding = 2f;

    [SerializeField] protected EnemyCategory EC; //Used for special perks from the player
    [SerializeField] protected FactionsEnum factionEnum;
    [SerializeField] protected Collider BodyCollider; //Only Add if body collider is seperate from the hitbox
    [SerializeField] private string EnemyTypeName;
    [SerializeField] private int exp_reward = 100;
    [SerializeField] protected float destroyAfterDeathDelay = 60f;
    [SerializeField] protected bool stunable = true;
    [SerializeField] protected bool knockbackable = true;
    [SerializeField] protected float MaxMovementForce;
    [SerializeField] protected float NewEnemyAcquireTime = 1f;


    [SerializeField] protected float expDamageThreshold = .25f; //Used for starting combat too


    [SerializeField] protected GameObject FlagRef_Death = null;

    protected CustomReputation CustomRep = CustomReputation.Standard; 

    protected bool AIenabled;
    protected bool NormalMovement = true;

    public GameObject player;
    protected Transform Current_Target;

    protected FactionLogic FL;
    protected QuestsHolder QH;
    protected Rigidbody rB;
    protected EnemyAnimationUpdater animationUpdater;
    protected Animator animator;
    protected NavMeshAgent agent;
    protected Health health;
    protected Transform Hitbox;
    protected EnemyHealthBar EHealBar;
    protected Zone_Flags ZF;

    // Roam logic
    protected float roam_time_tracker = 0f;
    protected bool roam = false;
    protected Transform Spawner_Transform = null;
    private int SpawnerArrayIter = -1;
    // Roam logic

    protected float original_speed;
    protected float PlayerPercentDamage = 0f;

    protected bool cc_immune;
    protected float NewTarget = 0f;
    protected float StunRelease;
    protected float timer;
    protected Transform deadEnemyParent;

    protected Vector3 direction; //Used by multiple funcs
    protected Vector3 mod_direction; //Used by multiple funcs


    //LOGIC CHANGING VARS
    protected float MAX_ANGLE = -1f;
    protected float NORMAL_ROT_SPEED = 180f;
    //




    //anchor logic
    private Vector3 anchor;
    private Transform anchorTerrain;
    //anchor logic

    private float dist_to_dest;
    private NavMeshPath NMP;
    private AI_Master AI_master;
    private Vector3 Current_Dest;

    ////////////////////////////////////Access Vars
    public bool Return_wasSpawned()
    {
        return Spawner_Transform != null;
    }
    
    public string Return_EnemyTypeName()
    {
        return EnemyTypeName;
    }
    public void Set_exp_reward(int xp_in)
    {
        exp_reward = xp_in;
    }
    public CustomReputation Return_customReputation()
    {
        return CustomRep;
    }
    public void Set_customReputation(CustomReputation customRep_in)
    {
        CustomRep = customRep_in;
    }
    public void PlayerDamageTaken(float fractionTaken)
    {
        PlayerPercentDamage += fractionTaken;
        if(PlayerPercentDamage > expDamageThreshold)
        {
            Set_customReputation(CustomReputation.PlayerEnemy);
        }
    }
    public FactionsEnum Return_FactionEnum()
    {
        return factionEnum;
    }
    public Transform Return_Current_Target()
    {
        return Current_Target;
    }
    public bool Return_AIenabled()
    {
        return AIenabled & Hitbox.tag != "DeadEnemy";
    }

    public bool isDead()
    {
        return Hitbox.tag == "DeadEnemy";
    }

    public EnemyAnimationUpdater Return_EAU()
    {
        return animationUpdater;
    }
    ////////////////////////////////////Access Vars


    ////////////////////////////////////Public Usable Functions
    public virtual void SpawnEnemy(FactionsEnum fac, bool Roam_in, Transform Spawner_Transform_in, int SpawnerArrayIter_in)
    {
        SwitchFaction(fac, false);
        roam = Roam_in;
        Spawner_Transform = Spawner_Transform_in;
        SpawnerArrayIter = SpawnerArrayIter_in;
    }

    public void EnableAI(bool Enabled_by_Rally) //Do if damaged or an enemy comes into range... DO NOT PROPAGATE
    {
        if (Hitbox.tag == "DeadEnemy" || AIenabled)
        {
            return;
        }

        AIEnable_extraLogic(Enabled_by_Rally);

        AIenabled = true;
        agent.speed = original_speed;
        EHealBar.Enable();
    }

    public virtual void EnemyKnockback(Vector3 Force)
    {
        if (cc_immune || !knockbackable)
        {
            return;
        }
        animationUpdater.PlayBlockingAnimation("take_damage", true); //Knockback duration determined by animation length
        rB.velocity = new Vector3(0f, 0f, 0f);
        rB.AddForce(Force);
    }

    public virtual void EnemyStun(float duration)
    {
        if (cc_immune || !stunable)
        {
            return;
        }
        StunRelease = timer + duration;
    }

    public void Death(bool ignore_logic = false)
    {
        if(Hitbox.tag != "DeadEnemy") //Call only once
        {
            Death_extraLogic();

            if (!ignore_logic)
            {
                if (PlayerPercentDamage >= expDamageThreshold)
                {
                    player.GetComponent<PlayerStats>().AddEXP(exp_reward);
                    //QH.CheckGenericKillObjectives(transform.gameObject, new Vector2(transform.position.x, transform.position.y));
                }

                if (Spawner_Transform)
                {
                    Spawner_Transform.GetComponent<Spawner>().ChildDied(SpawnerArrayIter);
                }

                if (FlagRef_Death)
                {
                    ZF.SetFlag(FlagRef_Death);
                }
            }


            // Freeze the position and remove collision of the enemy
            rB.constraints = RigidbodyConstraints.FreezeAll;
            rB.angularVelocity = Vector3.zero;
            animationUpdater.PlayAnimation("death", false, true);
            EHealBar.Disable();

            // Makes it so the dead enemy cannot be targeted like an alive one
            Hitbox.tag = "DeadEnemy";
            agent.enabled = false;
            Hitbox.GetComponent<Collider>().enabled = false;
            if (BodyCollider)
            {
                BodyCollider.enabled = false;
            }

            transform.parent = deadEnemyParent;
            Destroy(gameObject, destroyAfterDeathDelay);
        }
    }

    public void SwitchFaction(FactionsEnum FL_in, bool test_for_enemies)
    {
        factionEnum = FL_in;
        string FactionName = STARTUP_DECLARATIONS.FactionsEnumReverse[(int)factionEnum];

        if (gameObject.activeInHierarchy)
        {
            Transform NPCHolder = GameObject.Find(FactionName).transform;
            transform.parent = NPCHolder;
        }

        if (test_for_enemies)
        {
            EnableAI(false); //Here so it can check if enemys are already in range
        }
    }

    public void AI_Master_Interface()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.CalculatePath(Current_Dest, NMP);
            agent.SetPath(NMP);
        }
    }
    ////////////////////////////////////Public Usable Functions


    ////////////////////////////////////Protected Virtual Functions
    protected virtual void Awake()
    {
        cc_immune = false;
        timer = 0f;
        player = GameObject.Find("Player");
        Hitbox = transform.Find("Hitbox");
        if (!Hitbox)
        {
            Debug.Log(gameObject);
        }

        deadEnemyParent = GameObject.Find("_DEADNPC_").transform;

        AIenabled = false;
        QH = GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>();
        rB = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        animationUpdater = GetComponentInChildren<EnemyAnimationUpdater>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        health = GetComponentInChildren<Health>();
        EHealBar = GetComponentInChildren<EnemyHealthBar>();
        FL = GameObject.Find("NPCs").GetComponent<FactionLogic>();
        NMP = new NavMeshPath();
        AI_master = FindObjectOfType<AI_Master>();
        ZF = FindObjectOfType<Zone_Flags>();

        agent.enabled = true;
        original_speed = agent.speed;
    }

    protected void AgentMovementFunc(Vector3 dest, bool HighPriority)
    {
        Current_Dest = dest;
        dist_to_dest = new Vector2(transform.position.x - dest.x, transform.position.z - dest.z).magnitude;
        AI_master.Request(gameObject, (HighPriority && dist_to_dest < 30)); //HiPri if both HiPriority and near dest
    }

    protected virtual void AIFunc()
    {
        AgentMovementFunc(Current_Target.position, true);
    }

    protected virtual void RoamingFunc()
    {
        agent.speed = original_speed / 2;
        if (roam_time_tracker <= 0)
        {
            roam_time_tracker = 10 + Random.Range(-5, 5);
            Vector3 roam_dest = Spawner_Transform.position;
            roam_dest.x += Random.Range(-50f, 50f);
            roam_dest.z += Random.Range(-50f, 50f);
            AgentMovementFunc(roam_dest, false);
        }
        else
        {
            roam_time_tracker -= Time.fixedDeltaTime;
        }
    }

    protected virtual void EnemyBasicAnimation()
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), NORMAL_ROT_SPEED * Time.fixedDeltaTime);
        if(rB.velocity.magnitude < 0.1f)
        {
            animationUpdater.PlayAnimation("idle");
        }
        else
        {
            animationUpdater.PlayAnimation("Blend Tree");
        }
    }

    protected virtual bool CustomFixedFunc()
    {
        return false;
    }

    protected virtual void AIDisableFunc()
    {

    }

    protected virtual void Death_extraLogic()
    {

    }

    protected virtual void AIEnable_extraLogic(bool Enabled_by_Rally)
    {        
        if (!Enabled_by_Rally)
        {
            FL.RallyAllies(factionEnum, transform, CustomRep);
        }
    }
    ////////////////////////////////////Protected Virtual Functions


    ////////////////////////////////////Protected Fixed Functions
    protected virtual void EnemyMovement() //-1 means no max
    {
        if (direction.magnitude > 1) //Use direction not mod_direction for agent
        { 
            agent.nextPosition = (transform.position + agent.nextPosition) / 2;
        }

        mod_direction = mod_direction.normalized;

        float temp_speed;
        if (dist_to_dest > 1)
        {
            temp_speed = agent.speed;
        }
        else
        {
            temp_speed = agent.speed * dist_to_dest / 1; //Slowdown near dest
        }

        mod_direction = (mod_direction * temp_speed - rB.velocity);

        Vector3 currentforce = new Vector3();
        if (mod_direction.magnitude > 1f)
        {
            mod_direction = mod_direction.normalized;
            currentforce = MaxMovementForce * mod_direction;
        }
        else
        {
            currentforce = MaxMovementForce * mod_direction;
        }


        rB.AddForce(new Vector3(currentforce.x, 0, currentforce.z));
    } //This will always follow the agent
      ////////////////////////////////////Protected Fixed Functions


    protected virtual void FixedUpdate()
    {
        if (Hitbox.tag == "DeadEnemy")
        {
            return;
        }

        AnchorLogic();

        animator.SetFloat("MoveSpeed", rB.velocity.magnitude); //Can be overwritten by humanoids in CustomFixedFunc... MUST BE ABOVE
        bool breakVar = CustomFixedFunc();
        if (breakVar)
        {
            return;
        }

        timer += Time.fixedDeltaTime;

        if (AIenabled)
        {
            if (!Current_Target || timer >= NewTarget)
            {
                Current_Target = FL.FindEnemy(factionEnum, transform, CustomRep); //TODO Use collider prob
                //Debug.Log(Current_Target);
                if (Current_Target)
                {
                    NewTarget = timer + NewEnemyAcquireTime;
                    Current_Target = Current_Target.Find("Hitbox");
                }
            }

            if (!Current_Target) //DO NOT MAKE AN IF/ELSE, because the current_target can be set null above
            {
                AIenabled = false;
                EHealBar.Disable();
                AIDisableFunc();
            }
        }


        if (animationUpdater.ReturnBlocked() || StunRelease > timer) //knockback, stun, or self-disable
        {
            return;
        }
        else if (AIenabled)
        {
            AIFunc();
        }
        else if (roam)
        {
            RoamingFunc();
        }

        if (NormalMovement)
        {
            direction = agent.nextPosition - transform.position;
            mod_direction = direction; //needs to be above direction

            if (MAX_ANGLE != -1) //Changes mod_direction
            {
                float Dir_angle = Mathf.Atan2(direction.x, direction.z);
                float current_rot = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

                if (Dir_angle < 0)
                {
                    Dir_angle += 2 * Mathf.PI;
                }

                float tempAngle = Dir_angle - current_rot;


                if (tempAngle > Mathf.PI)
                {
                    tempAngle -= 2 * Mathf.PI;
                }
                else if (tempAngle < -Mathf.PI)
                {
                    tempAngle += 2 * Mathf.PI;
                }

                if (Mathf.Abs(tempAngle) > MAX_ANGLE)
                {
                    if (tempAngle > 0)
                    {
                        mod_direction = new Vector3(Mathf.Sin(current_rot + MAX_ANGLE), direction.y, Mathf.Cos(current_rot + MAX_ANGLE));
                    }
                    else
                    {
                        mod_direction = new Vector3(Mathf.Sin(current_rot - MAX_ANGLE), direction.y, Mathf.Cos(current_rot - MAX_ANGLE));
                    }    
                }
            }

            EnemyMovement();
            EnemyBasicAnimation();
        }
    }


    protected virtual void OnEnable()
    {
        SwitchFaction(factionEnum, false);
        anchor = transform.position;
        anchorTerrain = transform.parent; //might not be terrian, but this prob is fine
    }

    private void AnchorLogic()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + (2 * transform.up), -transform.up);
        if (Physics.Raycast(ray, out hit, 10f, 1 << LayerMask.NameToLayer("Terrain"))) //If terrain found below
        {
            anchor = transform.position;
            anchorTerrain = hit.transform;
        }
        else
        {
            transform.position = anchor;
            transform.parent = anchorTerrain;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!AIenabled)
        {
            if (other.tag == "Player")
            {
                if (FL.ReturnIsEnemy(Return_FactionEnum(), FactionsEnum.Player, Return_customReputation()))
                {
                    Current_Target = other.transform;
                    EnableAI(false);
                }
            }
            else if (other.tag == "BasicEnemy")
            {
                if (FL.ReturnIsEnemy(Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), Return_customReputation()))
                {
                    Current_Target = other.transform;
                    EnableAI(false);
                }
            }
        }
    }

    public virtual void AnimationCalledFunc0()
    {

    }

    public virtual void AnimationCalledFunc1()
    {

    }

    public virtual void AnimationCalledFunc2()
    {

    }
}
