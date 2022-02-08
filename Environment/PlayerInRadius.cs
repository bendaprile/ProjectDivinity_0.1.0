using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInRadius : MonoBehaviour
{
    [SerializeField] private float eject_range = 30f;
    public bool isTrue;
    private Transform player;
    
    void Start()
    {
        isTrue = false;
        player = GameObject.Find("Player").transform;
        Assert.IsNotNull(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isTrue = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isTrue = false;
        }
    }

    private int Out_of_range_iter = 0;
    private void FixedUpdate() //Remove enemies that are out of range, but didn't trigger the exit collider
    {
        if (isTrue)
        {
            if ((player.position - transform.position).magnitude > eject_range)
            {
                isTrue = false;
            }
        }
    }
}
