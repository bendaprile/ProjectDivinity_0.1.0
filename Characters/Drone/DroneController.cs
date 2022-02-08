using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private bool DroneActive = true;
    [SerializeField] private Light light_comp0 = null;
    [SerializeField] private Light light_comp1 = null;

    [SerializeField] private Transform focus_light_trans = null;

    [SerializeField] float distance = 2f;
    [SerializeField] float height = 4f;
    [SerializeField] float focusLightAngle = 20f;
    [SerializeField] UIController UIController;
    [SerializeField] Transform cameraTransform;

    private Transform playerTrans;
    private  Rigidbody rb;

    private SphereCollider SC;
    private QuestsHolder quests;

    private Light focus_light;

    private CursorLogic cursorLogic;
    /*
    public void ChangeDroneMode(bool enable)
    {
        DroneActive = enable;
    }


    private void Start()
    {
        playerTrans = GameObject.Find("Player").transform;
        focus_light = focus_light_trans.GetComponent<Light>();
        quests = GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>();
        SC = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        cursorLogic = GameObject.Find("Master Object").GetComponent<CursorLogic>();

        focus_light_trans.localEulerAngles = new Vector3(focusLightAngle, 0f, 0f);
    }

    void Update()
    {
        if(DroneActive)
        {
            light_comp1.transform.position = transform.position;
            ControlFlashlight();
            NormalFollow();
            rotation();
            ControlFocusLight();
        }
    }

    void NormalFollow()
    {
        Vector3 vector = playerTrans.position - transform.position;
        vector.y = 0f;

        float vecMag = vector.magnitude;

        if (vecMag > 10)
        {
            SC.enabled = false;
        }
        else
        {
            SC.enabled = true;
        }


        if (vecMag > distance)
        {
            Vector3 desired_movement = new Vector3(vector.x * 20f, 0f, vector.z * 20f) - rb.velocity; //Non-constant
            Vector3 forces = desired_movement / 3;
            rb.AddForce(forces);
        }

        Vector3 dronePos = transform.position;
        dronePos = Vector3.MoveTowards(dronePos, new Vector3(dronePos.x, playerTrans.position.y + height, dronePos.z), 0.1f);
        transform.position = dronePos;
    }

    void PauseMovement()
    {
        Vector3 newDronePos = new Vector3(playerTrans.position.x + 2.5f, playerTrans.position.y + 1f, playerTrans.position.z);
        newDronePos = Vector3.Lerp(transform.position, newDronePos, 0.05f);
        transform.position = newDronePos;
        transform.rotation = Quaternion.Lerp(transform.rotation, playerTrans.rotation, 0.1f);

        Vector3 newLightPos = Vector3.MoveTowards(light_comp1.transform.position, cameraTransform.position, 0.2f);
        light_comp1.transform.position = newLightPos;
    }

    // TODO: Handle Input through InputManager and not direct key references
    void ControlFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            light_comp0.enabled = !light_comp0.enabled;
            light_comp1.enabled = !light_comp1.enabled;
        }
    }
    
    void ControlFocusLight()
    {
        GameObject tempObj = quests.ReturnFocus();
        if (tempObj)
        {
            QuestTemplate tempScipt = tempObj.GetComponent<QuestTemplate>();

            Vector2 min_distance_vec = new Vector2();
            bool first = true;

            foreach ((Transform, float) loc in tempScipt.returnActiveLocs().Item2)
            {
                if(first)
                {
                    first = false;
                    min_distance_vec = new Vector2(loc.Item1.x - transform.position.x, loc.Item1.y - transform.position.z);
                }
                else
                {
                    Vector2 tempVec = new Vector2(loc.Item1.x - transform.position.x, loc.Item1.y - transform.position.z);
                    if(tempVec.magnitude < min_distance_vec.magnitude)
                    {
                        min_distance_vec = tempVec;
                    }
                }
            }

            if(!first)
            {
                float angle = Mathf.Atan2(min_distance_vec.x, min_distance_vec.y) * Mathf.Rad2Deg;
                float ver_angle = Mathf.Atan2(height, min_distance_vec.magnitude) * Mathf.Rad2Deg;
                float final_ver_angle;
                if (ver_angle > focusLightAngle)
                {
                    final_ver_angle = ver_angle;
                }
                else
                {
                    final_ver_angle = focusLightAngle;
                }
                focus_light.enabled = true;
                focus_light_trans.eulerAngles = new Vector3(final_ver_angle, angle, focus_light_trans.localEulerAngles.z);
            }
        }
        else
        {
            focus_light.enabled = false;
        }
    }

    private void rotation()
    {
        float angle;
        float rotation_speed;
        if (light_comp0.enabled)
        {
            angle = Mathf.Atan2(cursorLogic.ReturnMousePos().x - transform.position.x, cursorLogic.ReturnMousePos().z - transform.position.z) * Mathf.Rad2Deg;
            rotation_speed = 720 * Time.fixedDeltaTime;
        }
        else
        {
            angle = Mathf.Atan2(playerTrans.position.x - transform.position.x, playerTrans.position.z - transform.position.z) * Mathf.Rad2Deg;
            rotation_speed = 360 * Time.fixedDeltaTime;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), rotation_speed);
    }

    */
}
