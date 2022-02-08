using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDilation : Ability
{
    [SerializeField] private float duration = 4f;
    [SerializeField] private float ratio = .5f;


    private float originalTimeScale;

    private bool dilationActive;
    private float ending_countdown;

    TimeDilation()
    {
        abilityType = AbilityType.Spacetime;
    }

    protected override float AbilityEffectMult() //TODO
    {
        float Mult = base.AbilityEffectMult();
        return Mult;
    }

    void Start()
    {
        dilationActive = false;
        originalTimeScale = Time.timeScale;
    }

    protected override void Attack()
    {
        dilationActive = true;
        ending_countdown = duration * AbilityEffectMult();
        Time.timeScale = ratio * originalTimeScale;
    }


    // Update is called once per frame
    void Update()
    {
        if(dilationActive)
        {
            ending_countdown -= Time.deltaTime;
            if(ending_countdown <= 0)
            {
                dilationActive = false;
                Time.timeScale = originalTimeScale;
            }
        }
    }
}
