using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject Menu = null;
    [SerializeField] private GameObject HUD = null;
    [SerializeField] private GameObject Map = null;
    [SerializeField] private GameObject InteractiveObjectMenu = null;
    [SerializeField] private GameObject DialogueMenu = null;
    [SerializeField] private CinemachineBrain cinemachineBrain = null;
    [SerializeField] private GameObject compass = null;

    private Transform Player;

    //External Use
    public UI_Mode current_UI_mode = UI_Mode.Normal;
    public Vector3 Centerpoint;
    //External Use

    private List<Transform> npcTransforms = new List<Transform>();

    private float originalTimeScale;
    private bool mapOpen = false;

    private PlayerStats playerStats;
    private PlayerAnimationUpdater playerAnimationUpdater;
    private CameraStateController camStateController;
    private HumanoidMaster CurrentMerchant = null;
    private PlayerMaster PM;

    // Start is called before the first frame update
    void Start()
    {
        Menu.SetActive(false);
        originalTimeScale = Time.timeScale;

        Player = GameObject.Find("Player").transform;
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        playerAnimationUpdater = playerStats.GetComponentInChildren<PlayerAnimationUpdater>();
        camStateController = FindObjectOfType<CameraStateController>();
        PM = FindObjectOfType<PlayerMaster>();
    }


    // Update is called once per frame
    void Update()
    {
        if (PM.Return_isDead())
        {
            return;
        }

        // TODO: Handle Input through InputManager and not direct key references
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (current_UI_mode == UI_Mode.InteractiveMenu)
            {
                if (CurrentMerchant)
                {
                    CurrentMerchant.SetMovementLocked(false);
                }
                InteractiveObjectMenu.GetComponent<InteractiveObjectMenuUI>().DisablePanel();
                current_UI_mode = UI_Mode.Normal;
            }
            else if (current_UI_mode == UI_Mode.PauseMenu)
            {
                Menu.SetActive(false);
                Unpaused();
            }
            else if(current_UI_mode == UI_Mode.Normal)
            {
                Menu.SetActive(true);
                Paused();
            }
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (current_UI_mode == UI_Mode.Normal)
            {
                EnableDisableMap(!mapOpen);
            }
            else if (current_UI_mode == UI_Mode.PauseMenu)
            {
                Menu.GetComponent<MenuController>().WorldEnable();
            }
        }



        if(current_UI_mode == UI_Mode.DiaMenu)
        {
            Centerpoint = Player.position;

            for (int i = 0; i < npcTransforms.Count; ++i)
            {
                Centerpoint += npcTransforms[i].position;
            }
            Centerpoint /= (npcTransforms.Count + 1);

            for (int i = 0; i < npcTransforms.Count; ++i)
            {
                Vector3 direction = (Centerpoint - npcTransforms[i].position);
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                npcTransforms[i].rotation = Quaternion.RotateTowards(npcTransforms[i].rotation, Quaternion.Euler(new Vector3(0, angle, 0)), 270 * Time.unscaledDeltaTime);
            }
        }
    }

    public void EnableDisableMap(bool enable)
    {
        if (enable)
        {
            Map.SetActive(true);
            Map.GetComponentInChildren<Animator>().Play("In");
            mapOpen = true;
        }
        else if (Map.activeSelf)
        {
            mapOpen = false;
            Map.GetComponentInChildren<Animator>().Play("Out");
            StartCoroutine(TurnOffPanel(Map));
        }
    }

    public void OpenInteractiveMenu(GameObject container)
    {
        CurrentMerchant = container.GetComponentInParent<HumanoidMaster>(); //There will not always be this
        if (CurrentMerchant)
        {
            CurrentMerchant.SetMovementLocked(true);
        }
        Map.SetActive(false);
        playerAnimationUpdater.PlayAnimation("idle");
        current_UI_mode = UI_Mode.InteractiveMenu;
        InteractiveObjectMenu.SetActive(true);
        InteractiveObjectMenu.GetComponent<InteractiveObjectMenuUI>().Refresh(container);
    }

    public void ReturnMapLocation(Transform mapRect)
    {
        StartCoroutine(ReturnMapLocationCoroutine(mapRect));
    }

    private IEnumerator TurnOffPanel(GameObject panel)
    {
        yield return new WaitForSecondsRealtime(0.3f);

        if (!Map || !mapOpen)
        {
            panel.SetActive(false);
        }
    }

    private IEnumerator ReturnMapLocationCoroutine(Transform mapRect)
    {
        yield return new WaitForSecondsRealtime(0.3f);

        if (!mapOpen)
        {
            mapRect.localPosition = new Vector3(960f, 597.5f);
        }
    }

    public void add_dia_npc(Transform npc)
    {
        if (!npcTransforms.Contains(npc))
        {
            npcTransforms.Add(npc);
        }
    }


    Queue<Transform> DiaQueue = new Queue<Transform>();
    public void DialogueMenuBool(Transform DiaData = null) //WARNING Order is hyper sensitive here
    {
        if (DiaData != null)
        {
            if (current_UI_mode == UI_Mode.InteractiveMenu) //Leave interactive menu if someone comes to talk to you
            {
                InteractiveObjectMenu.GetComponent<InteractiveObjectMenuUI>().DisablePanel();
            }

            HumanoidMaster TempMaster = DiaData.GetComponentInParent<HumanoidMaster>();

            if (TempMaster)
            {
                TempMaster.SetMovementLocked(true);
                npcTransforms.Add(TempMaster.transform);
            }

            if (current_UI_mode == UI_Mode.DiaMenu)
            {
                DiaQueue.Enqueue(DiaData);
                return;
            }


            current_UI_mode = UI_Mode.DiaMenu;
            playerAnimationUpdater.PlayAnimation("idle");

            Map.SetActive(false);
            compass.SetActive(false);
            HUD.GetComponent<CanvasGroup>().alpha = 0;
            DialogueMenu.SetActive(true);
            DialogueMenu.GetComponent<Animator>().Play("Panel In");
            DialogueMenu.GetComponent<DiaParent>().SetupDia(DiaData);

            camStateController.SetCamState(CameraStateController.CameraState.DialogueMenuCam);
        }
        else
        {
            foreach(Transform trans in npcTransforms)
            {
                HumanoidMaster HM = trans.GetComponent<HumanoidMaster>();
                if (HM) //The Dia could be locking at a non-humanoid thing
                {
                    trans.GetComponent<HumanoidMaster>().SetMovementLocked(false);
                }
            }
            npcTransforms.Clear();

            if(camStateController.ReturnCameraState() == CameraStateController.CameraState.DialogueMenuCam) //Check here in case the player got moved by a world setup
            {
                camStateController.RevertCamToPreviousState(); //e.g. guard logic moving the player to jail (switching the state to building)
            }

            current_UI_mode = UI_Mode.Normal;
            compass.SetActive(true);

            if (DiaQueue.Count > 0)
            {
                DialogueMenuBool(DiaQueue.Dequeue());
            }
            else
            {

                DialogueMenu.GetComponent<Animator>().Play("Panel Out");
                StartCoroutine(TurnOffPanel(DialogueMenu));
                HUD.GetComponent<CanvasGroup>().alpha = 1;
            }

        }
    }

    void Paused()
    {
        compass.SetActive(false);
        cinemachineBrain.m_IgnoreTimeScale = true;
        cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        playerAnimationUpdater.SetUpdateMode(AnimatorUpdateMode.UnscaledTime);
        playerAnimationUpdater.PlayAnimation("idle");
        camStateController.SetCamState(CameraStateController.CameraState.PauseMenuCam);
        current_UI_mode = UI_Mode.PauseMenu; 


        playerAnimationUpdater.GetComponentInParent<Rigidbody>().velocity = Vector3.zero;
        playerAnimationUpdater.GetComponentInParent<Rigidbody>().angularVelocity = Vector3.zero;
        Time.timeScale = 0f;
        HUD.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Unpaused()
    {
        compass.SetActive(true);
        cinemachineBrain.m_IgnoreTimeScale = false;
        cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
        playerAnimationUpdater.SetUpdateMode(AnimatorUpdateMode.Normal);
        playerAnimationUpdater.PlayAnimation("idle");
        camStateController.RevertCamToPreviousState();
        current_UI_mode = UI_Mode.Normal;

        Time.timeScale = originalTimeScale;
        HUD.GetComponent<CanvasGroup>().alpha = 1;
    }
}
