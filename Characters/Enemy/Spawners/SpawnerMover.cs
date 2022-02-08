using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnerMover : MonoBehaviour
{
    [SerializeField] Vector3[] TravelPoints;
    private NavMeshAgent agent;
    private int iter = 0;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        UpdateDest();
    }

    void FixedUpdate()
    {
        if(agent.remainingDistance < 5)
        {
            UpdateDest();
        }
    }

    private void UpdateDest()
    {
        agent.destination = TravelPoints[iter];
        iter = (iter + 1) % TravelPoints.Length;
    }
}
