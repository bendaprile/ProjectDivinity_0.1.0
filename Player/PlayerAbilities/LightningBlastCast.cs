using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBlastCast : Ability
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float dps = 40f;

    private GameObject Lightning;
    private Transform LightningEnd;

    public bool activated;

    LightningBlastCast()
    {
        abilityType = AbilityType.Lightning;
    }


    void Start()
    {
        Lightning = transform.Find("SmallLightning").gameObject;
        LightningEnd = Lightning.transform.Find("LightningEnd");
        energy = GameObject.Find("Player").GetComponent<Energy>();

        Lightning.SetActive(false);
        activated = false;
    }

    public override void AttemptAttack() //Override this one because the energy is not used up front
    {
        activated = !activated;
        Lightning.SetActive(activated);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(activated)
        {
            if (!energy.Drain_ES(true, energyCost * Time.fixedDeltaTime))
            {
                activated = !activated;
                Lightning.SetActive(activated);
            }

            RaycastHit hitray;
            int layerMask = (LayerMask.GetMask("Player") | LayerMask.GetMask("Ignore Raycast"));
            layerMask = ~layerMask;

            LightningEnd.localPosition = new Vector3(0, 0, 10);

            if (Physics.Raycast(transform.position, transform.forward, out hitray, range, layerMask))
            {
                LightningEnd.position = hitray.collider.transform.position;
                if (hitray.collider.tag == "BasicEnemy")
                {
                    (hitray.collider.gameObject).GetComponent<Health>().take_damage(dps * Time.fixedDeltaTime * AbilityEffectMult(), DamageSource.FinesseBased, true, DT: DamageType.Regular, isDoT: true);
                }
            }
        }
    }
}
