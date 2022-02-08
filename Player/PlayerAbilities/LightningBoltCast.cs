using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltCast : Ability
{
    public GameObject lightningBolt;
    [SerializeField] private float damage = 80f;
    [SerializeField] private float radius = 4f;
    [SerializeField] private float delay = 1f;

    LightningBoltCast()
    {
        abilityType = AbilityType.Lightning;
    }

    protected override void Attack()
    {
        GameObject clone = Instantiate(lightningBolt, SetPos(), transform.rotation, PlayerProjectiles).gameObject;
        LightningBoltProjectile LBP = clone.GetComponent<LightningBoltProjectile>();
        LBP.GenericSetup(damage, radius, delay);
    }

    // TODO: Handle for controllers
    private Vector3 SetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = (LayerMask.GetMask("Terrain"));
        RaycastHit hitray;
        Vector3 mousePos3d = new Vector3(0f, 0f, 0f);

        if (Physics.Raycast(ray, out hitray, Mathf.Infinity, layerMask))
        {
            mousePos3d = hitray.point;
        }

        return mousePos3d;
    }
}
