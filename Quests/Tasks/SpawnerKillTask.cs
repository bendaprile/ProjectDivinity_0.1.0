using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerKillTask : QuestTask
{
    [SerializeField] private GameObject Spawner;
    [SerializeField] private bool Enable_or_SelfSpawn = false; //(COLLIDER NOT CODE) Must be true if the Spawner code is attached to the quest GameObject
    [SerializeField] private bool DisableSpawnerAfter = false;


    protected override void initialize()
    {
        Spawner.GetComponent<Spawner>().AttachToQuest(Enable_or_SelfSpawn, gameObject); //TODO
    }

    protected override void TaskCleanupLogic()
    {
        if (DisableSpawnerAfter)
        {
            Spawner.SetActive(false);
            Spawner.GetComponent<Spawner>().KillSpawnerChildren(false);
        }
    }
}
