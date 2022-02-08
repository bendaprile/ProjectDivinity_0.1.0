using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereThrowingEnemy : EnemyAbility
{
    [SerializeField] private GameObject lightningSphere = null;
    [SerializeField] private string BlockingCastAnimation = "";

    [SerializeField] private bool require_forward = true;
    [SerializeField] private List<float> ShotAngles = new List<float>(); //In Degrees

    [SerializeField] private float radius = 0f;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float max_duration = 0f;
    [SerializeField] private float dps = 0f;
    [SerializeField] private int max_targets = 3;
    [SerializeField] private float MaintainSpeed = 0;

    public override bool CheckCast()
    {
        if (clean_LoS(require_forward))
        {
            set_BAiU(true);
            ETM.Return_EAU().PlayBlockingAnimation(BlockingCastAnimation);
            return true;
        }
        return false;
    }


    public override void CastMechanicsForce()
    {
        set_BAiU(false);
        Vector3 mod_transform = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        foreach(float angle in ShotAngles)
        {
            GameObject clone = Instantiate(lightningSphere, mod_transform, transform.rotation, EnemyProjectiles);
            Rigidbody RB = clone.GetComponent<Rigidbody>();
            LightningSphereProjectile LSP_Script = clone.GetComponentInChildren<LightningSphereProjectile>();

            Vector3 Dir = new Vector3(Mathf.Sin((transform.eulerAngles.y  + angle) * Mathf.Deg2Rad), 0f, Mathf.Cos((transform.eulerAngles.y + angle) * Mathf.Deg2Rad));

            RB.velocity = Dir * speed;

            LSP_Script.EnemySetup(FL, ETM);
            LSP_Script.GenericSetup(MaintainSpeed, dps, max_targets, max_duration, radius);

            clone.transform.Find("Sphere").GetComponent<SphereCollider>().isTrigger = true;
        }
    }
}
