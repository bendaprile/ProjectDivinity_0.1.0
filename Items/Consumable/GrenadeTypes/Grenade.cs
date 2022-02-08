using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Grenade : MonoBehaviour
{
    [SerializeField] DamageType DT = DamageType.Regular;
    [SerializeField] float damage = 25f;
    [SerializeField] float cookTime = 2f;
    [SerializeField] float force = 50f;
    [SerializeField] float heightOffset = 3f;
    [SerializeField] GameObject Explosion = null;
    [SerializeField] ColliderChild CC = null;
    [SerializeField] ColliderChildLayer Slow = null;

    private Transform ProjectileParent;
    private Rigidbody rb;


    public void Setup()
    {
        rb = GetComponent<Rigidbody>();
        ProjectileParent = transform.parent;
        StartCoroutine("CoRoutine");
    }

    IEnumerator CoRoutine()
    {
        yield return new WaitForSeconds(cookTime);
        GameObject temp = Instantiate(Explosion, ProjectileParent);
        temp.transform.position = new Vector3(transform.position.x, transform.position.y + heightOffset, transform.position.z);
        Destroy(temp, 5f);

        foreach(Collider col in CC.TriggerList)
        {
            Vector3 force_dir = col.transform.position - transform.position;
            force_dir *= force;
            col.GetComponent<EnemyHealth>().take_damage(damage, DamageSource.VigorBased, true, true, force_dir, DT: DT);
        }
         
        Destroy(gameObject);
    }

    private void FixedUpdate()     //This is to fix the issue of the grendade falling through terrrain
    { 
        if (Slow.TriggerList.Count > 0 && rb.velocity.y < -1)
        {
            rb.velocity = new Vector3(rb.velocity.x, -1, rb.velocity.z);
        }
    }
}
