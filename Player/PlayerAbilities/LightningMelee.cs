using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningMelee : Ability
{
    public GameObject Lightning;
    [SerializeField] float damage = 25f;
    [SerializeField] float damage_duration = .2f;
    [SerializeField] float timeout = 10f;

    [SerializeField] int charge_increment = 7;
    [SerializeField] int max_targets = 3;

    private Transform LightningStartPos;


    public int CurrentCharges;
    private float dps;

    private int assignment_counter;
    private float time_keeper;


    private float[] TimerArray;
    private Collider[] ColArray;
    private GameObject[] lightningArray;
    private Transform[] lightningArrayStarting;
    private Transform[] lightningArrayEnding;

    LightningMelee()
    {
        abilityType = AbilityType.Lightning;
    }

    void Start()
    {
        time_keeper = 0f;
        CurrentCharges = 0;
        assignment_counter = 0;
        dps = damage / damage_duration;

        PlayerProjectiles = GameObject.Find("PlayerProjectiles").transform;


        TimerArray = new float[max_targets];
        ColArray = new Collider[max_targets];

        lightningArray = new GameObject[max_targets];
        lightningArrayStarting = new Transform[max_targets];
        lightningArrayEnding = new Transform[max_targets];
        for (int i = 0; i < max_targets; i++)
        {
            TimerArray[i] = 0;
            lightningArray[i] = Instantiate(Lightning, transform.position, Quaternion.identity);
            lightningArray[i].transform.parent = gameObject.transform;

            lightningArrayStarting[i] = lightningArray[i].transform.Find("LightningStart");
            lightningArrayEnding[i] = lightningArray[i].transform.Find("LightningEnd");
        }
    }

    protected override void Attack()
    {
        LightningStartPos = GameObject.Find("Player").transform.FindDeepChild("RightHand"); //Player is not generated at start
        time_keeper = 0f;
        CurrentCharges = charge_increment;
    }


    public void MeleeAttack(Collider col)
    {
        ColArray[assignment_counter] = col;
        if (CurrentCharges > 0)
        {
            TimerArray[assignment_counter] = damage_duration;
        }
        else
        {
            TimerArray[assignment_counter] = 0;
        }
        assignment_counter = (assignment_counter + 1) % max_targets;
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(time_keeper >= timeout)
        {
            CurrentCharges = 0;
        }
        time_keeper += Time.fixedDeltaTime;


        for (int i = 0; i < max_targets; i++)
        {
            if (CurrentCharges > 0)
            {
                lightningArrayStarting[i].position = LightningStartPos.position;
                lightningArray[i].SetActive(true);

                if (time_keeper > (Time.fixedDeltaTime / 2)) //Start a frame later to fix visual glitch
                {
                    lightningArray[i].GetComponent<LineRenderer>().enabled = true;
                }


                if (TimerArray[i] > 0)
                {
                    TimerArray[i] -= Time.fixedDeltaTime;

                    if(TimerArray[i] <= 0)
                    {
                        --CurrentCharges;
                    }

                    lightningArrayEnding[i].position = ColArray[i].transform.position;
                    ColArray[i].gameObject.GetComponent<Health>().take_damage(dps * Time.fixedDeltaTime * AbilityEffectMult(), DamageSource.FinesseBased, true, DT: DamageType.Regular, isDoT: true);

                }
                else
                {
                    Vector3 endpos = LightningStartPos.position;
                    endpos += new Vector3(.5f * Random.value - .25f, .5f * Random.value - .25f, .5f * Random.value - .25f);
                    lightningArrayEnding[i].position = endpos;
                }
            }
            else
            {
                lightningArray[i].GetComponent<LineRenderer>().enabled = false;
                lightningArray[i].SetActive(false);
            }
        }
    }
}
