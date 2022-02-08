using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HomingScriptedBullet : MonoBehaviour
{
    [SerializeField] private Transform TargetHitbox;
    [SerializeField] private float damage = 10000f;
    [SerializeField] private float speed = 40f;
    [SerializeField] private bool unscaledtime = true;

    private void OnEnable()
    {
        Assert.IsTrue(TargetHitbox.tag == "BasicEnemy");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform == TargetHitbox)
        {
            other.gameObject.GetComponent<Health>().take_damage(damage, DamageSource.CerebralBased); //DS shouldnt matter if its a 1-hit KO
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Vector3 dir = TargetHitbox.position - transform.position;
        Debug.Log(dir.magnitude);
        dir *= speed / dir.magnitude;

        if (unscaledtime)
        {
            dir *= Time.unscaledDeltaTime;
        }
        else
        {
            dir *= Time.deltaTime;
        }

        transform.position += dir;

        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg, 90);
    }
}
