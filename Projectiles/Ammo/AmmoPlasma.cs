using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPlasma : AmmoMaster
{
    public void PlasmaSetup(int dam, float size, bool destEnem = true)
    {
        damage = dam;
        GetComponent<Transform>().localScale = new Vector3(size, size, size);
        destroyOnEnemy = destEnem;
        Destroy(gameObject, 5f);
    }
}
