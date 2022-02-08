using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PlayerMaster : MonoBehaviour
{
    [SerializeField] private Transform RespawnPos;
    [SerializeField] private LightBuildingAreaController[] respawnLighting;
    
    private Volume deathFog;
    private Collider rollingCollider;
    private PlayerMovement playerMovement;
    private PlayerAnimationUpdater animationUpdater;
    private WeaponController weaponController;
    private AbilitiesController abilitiesController;
    private ConsumableController consumableController;
    private PlayerHealth PH;
    private Energy PE;
    private CameraStateController CSC;
    private MultiRoomBuildingController Start_MRBC;

    private UIController UIControl;
    private CursorLogic CL;


    private GameObject prom;
    private Transform deathStarting_dia;


    public static InputManager inputActions;
    private bool PlayerControl = true;
    private bool is_dead = false;


    private void Awake()
    {
        inputActions = new InputManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Core functionality
        playerMovement = GetComponent<PlayerMovement>();
        weaponController = GetComponentInChildren<WeaponController>();
        animationUpdater = GetComponentInChildren<PlayerAnimationUpdater>();
        abilitiesController = GetComponentInChildren<AbilitiesController>();
        consumableController = GetComponentInChildren<ConsumableController>();
        PH = GetComponentInChildren<PlayerHealth>();
        PE = GetComponentInChildren<Energy>();
        CL = FindObjectOfType<CursorLogic>();
        CSC = FindObjectOfType<CameraStateController>();
        deathFog = GameObject.Find("Death Fog Volume").GetComponent<Volume>();

        prom = GameObject.Find("Prometheus UNIQUE");
        deathStarting_dia = GameObject.Find("Start Death Unique").transform;

        if (GameObject.Find("Wasteland Facility UNIQUE"))
        {
            Start_MRBC = GameObject.Find("Wasteland Facility UNIQUE").GetComponent<MultiRoomBuildingController>();
        }

        UIControl = GameObject.Find("UI").GetComponent<UIController>();
        //Debug.Log(UIControl);
        rollingCollider = GameObject.Find("RollingCollider").GetComponent<Collider>();
    }


    public void Set_PlayerControl(bool set_in) //Independent of UI_Control
    {
        PlayerControl = set_in;
    }

    public bool Return_isDead()
    {
        return is_dead;
    }


    private float WeaponLockOut = .5f;
    private float current_WeaponLockOut = .5f;
    private void Update() //THE ORDER OF THESE MATTER, UpdatePlayerState needs to be before handle weapon and handleabilites so that a roll can start
    {
        if (is_dead)
        {
            return;
        }

        if (UIControl.current_UI_mode == UI_Mode.Normal)
        {
            if (PlayerControl)
            {
                // Player movement
                playerMovement.UpdatePlayerState(); //must be above abilites and weapons

                //Ability Controller
                abilitiesController.HandleAbilities(1f);

                // Item Controller
                if (current_WeaponLockOut > 0)
                {
                    current_WeaponLockOut -= Time.deltaTime;
                }
                else
                {
                    weaponController.HandleWeapon();
                }

                //Consumable Controller
                consumableController.HandleConsumables();
            }

            // Player animation
            animationUpdater.UpdateAnimation();
        }
        else
        {
            current_WeaponLockOut = WeaponLockOut;

            if (UIControl.current_UI_mode == UI_Mode.PauseMenu)
            {
                playerMovement.PauseAimTowardCamera();
                animationUpdater.UpdateAnimationPauseMenu();
            }
            else if (UIControl.current_UI_mode == UI_Mode.DiaMenu)
            {
                /*
                if (!UIControl.npcTransform)
                {
                    return;
                }
                */
                playerMovement.DialogueMenuAimTowards(UIControl.Centerpoint);
            }
        }
    }

    private void FixedUpdate()
    {
        if (PlayerControl && !is_dead && UIControl.current_UI_mode == UI_Mode.Normal)
        {
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            playerMovement.Move();
            CL.GetPosition();
            //Vector2 movementInput = inputActions.Movement.Move.ReadValue<Vector2>();
        }
        else
        {
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        CL.InteractiveObjectFunction(UIControl.current_UI_mode == UI_Mode.Normal); //When not in Normal mode, stop all interactive objects

        rollingCollider.enabled = playerMovement.GetMoveState() == MoveState.Rolling;
    }



    public void PlayerDeath(bool normalDeath)
    {
        if (is_dead) //Don't call when dead
        {
            return;
        }
        is_dead = true;

        if (normalDeath)
        {
            StartCoroutine(DeathCoroutine());
        }
        else
        {
            StartCoroutine(RebirthCoroutine());
        }
    }

    IEnumerator DeathCoroutine()
    {
        CSC.SetCamState(CameraStateController.CameraState.DeathCam);
        FindObjectOfType<QuestsHolder>().CheckPlayerDeath();
        animationUpdater.PlayAnimation("death");

        float iter_intro = 0;
        PH.set_is_immortal(true);
        while (iter_intro < 8)
        {
            iter_intro += Time.deltaTime;
            deathFog.weight = iter_intro / 6; //Full volume before max dist
            yield return null;
        }
        prom.GetComponentInChildren<DiaRoot>().ModifyStarting(deathStarting_dia);
        StartCoroutine(RebirthCoroutine());
    }

    IEnumerator RebirthCoroutine()
    {
        deathFog.weight = 1;
        transform.position = RespawnPos.position;
        animationUpdater.PlayAnimation("idle");
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame(); //2 frames are needed to handle effect that require the player is moved.
        //e.g. (AudioOverrideCollider needs to play General before new special audio is called below)

        FindObjectOfType<BuildingControllerMaster>().ForceInsideBuilding(Start_MRBC, 1);
        foreach (LightBuildingAreaController respawnLight in respawnLighting)
        {
            respawnLight.ForceTriggerEnter();
        }
        PH.OnDeathFunc();
        PE.OnDeathFunc();

        float iter = 0;
        while (iter < 4)
        {
            iter += Time.deltaTime;
            deathFog.weight = 1 - (iter / 4);
            yield return null;
        }

        deathFog.weight = 0;
        PH.set_is_immortal(false);
        is_dead = false;

        GameObject.Find("Prometheus UNIQUE").GetComponent<HumanoidMaster>().Set_ControlMode(NPC_Control_Mode.WalktoPlayer_dia);
    }


    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
