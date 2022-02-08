using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Spawner : MonoBehaviour
{
    [SerializeField] protected List<Transform> enemyPool = new List<Transform>();
    [SerializeField] protected FactionsEnum Faction = FactionsEnum.Rogue;
    [SerializeField] protected List<int> max_enemy = new List<int>(); //Leave blank for level based enemies

    [SerializeField] private bool SpawnerEnabled = true;
    [SerializeField] private bool roam = false;
    [SerializeField] private float RefreshDelay = -1;

    
    private float last_Refresh = 0;
    private float enemies_loaded = 0;

    private bool Initalized = false;

    protected PlayerStats PS;

    private QuestsHolder QH;
    private List<int> Current_EnemyCount = new List<int>();

    private GameObject QuestRef;
    private List<GameObject> Children = new List<GameObject>();

    public (bool, float) dataDump()
    {
        float current_enemies = Return_EnemyCount();
        if(RefreshDelay != -1)
        {
            current_enemies += (Time.time - last_Refresh) / RefreshDelay;
        }
        return (SpawnerEnabled, current_enemies);
    }

    public void External_Load(bool enabled, float Curent_enemies) //Performed before Initalize
    {
        //Debug.Log((gameObject, Curent_enemies));
        SpawnerEnabled = enabled;
        enemies_loaded = Curent_enemies;
    }

    public virtual void External_Initalize(bool set_to_max) //Will happen after load
    {
        //Debug.Log(("Init", gameObject, set_to_max));
        Assert.IsTrue(max_enemy.Count == 0 || max_enemy.Count == enemyPool.Count);
        Initalized = true;
        PS = FindObjectOfType<PlayerStats>();
        QH = FindObjectOfType<QuestsHolder>();
        SetMaxEnemy();

        if (set_to_max)
        {
            int sum_max = 0;
            for (int i = 0; i < max_enemy.Count; ++i)
            {
                sum_max += max_enemy[i];
            }
            enemies_loaded = sum_max;
        }

        //gameObject.activeInHierarchy is really only for editor so that zones can be disabled
        if (SpawnerEnabled && gameObject.activeInHierarchy) //This won't be run OnEnable when loading because Initalized will be false
        {
            Replenish_enemies();
        }
    }

    public void AttachToQuest(bool en, GameObject QuestRef_in)
    {
        QuestRef = QuestRef_in;
        if (en)
        {
            SpawnerEnabled = true;
            Replenish_enemies();
        }
        else if (Return_EnemyCount() == 0) //DO not check if just enabling
        {
            QH.CheckExternalTaskCompletion(QuestRef);
        }
    }

    public int Return_EnemyCount()
    {
        if (!Initalized)
        {
            return -1;
        }

        int total = (int)enemies_loaded; //This line should only affect disabled spawners
        for(int i = 0; i < Current_EnemyCount.Count; ++i)
        {
            total += Current_EnemyCount[i];
        }

        return total;
    }

    public bool KillSpawnerChildren(bool inactive_only)
    {
        bool allKilled = true;
        for (int i = Children.Count - 1; i >= 0; --i)
        {
            if (Children[i])
            {
                EnemyTemplateMaster ETM = Children[i].GetComponent<EnemyTemplateMaster>();
                if (!inactive_only || !ETM.Return_AIenabled() || !Children[i].activeInHierarchy)
                {
                    Children[i].GetComponent<EnemyTemplateMaster>().Death(true);
                }
                else if(!Children[i].GetComponent<EnemyTemplateMaster>().isDead())
                {
                    allKilled = false;
                }
            }
        }
        return allKilled;
    }

    public void ChildDied(int ArrayIter) //This is so sad - Zach 2/2/2021
    {
        Current_EnemyCount[ArrayIter] -= 1;

        int total = 0;
        for (int i = 0; i < Current_EnemyCount.Count; ++i)
        {
            total += Current_EnemyCount[ArrayIter];
        }

        if (total == 0)
        {
            QH.CheckExternalTaskCompletion(QuestRef);
        }
    }

    ///Private//////////////////////////////////////////
    private void OnEnable()
    {
        if(Initalized && SpawnerEnabled) //This won't be run when loading because Initalized will be false
        {
            Replenish_enemies(); //This is only to add enemies that were replenished. Not Setup
        }
    }

    ///Protected//////////////////////////////////////////
    protected virtual void SetMaxEnemy()
    {
        if (max_enemy.Count != 0)
        {
            return;
        }

        int max_difficultly_points = PS.returnLevel() + 10;

        for(int i = 0; i < enemyPool.Count; ++i)
        {
            max_enemy.Add((int)((max_difficultly_points / enemyPool.Count) / enemyPool[i].GetComponent<EnemyTemplateMaster>().DifficultyPoints));
        }
    }

    protected virtual void AdditionalSetup(int i, Transform clone)
    {

    }


    private void Replenish_enemies()
    {
        int sum_max = 0;
        for (int i = 0; i < max_enemy.Count; ++i)
        {
            sum_max += max_enemy[i];
        }

        int enemies_to_refresh;
        if (RefreshDelay == -1)
        {
            enemies_to_refresh = (int)enemies_loaded;
        }
        else
        {
            enemies_to_refresh = (int)(enemies_loaded + ((Time.time - last_Refresh) / RefreshDelay)); //Number of enemies to refresh according to the time
        }
        enemies_loaded = 0; //Make sure enemies are not loaded each refresh

        if (enemies_to_refresh == 0)
        {
            return;
        }

        int childSqrtRoot = (int)Mathf.Sqrt(sum_max);
        if (Current_EnemyCount.Count == 0)
        {
            for (int i = 0; i < enemyPool.Count; ++i)
            {
                Current_EnemyCount.Add(0);
            }
        }
        int sum_current = 0;
        for (int i = 0; i < Current_EnemyCount.Count; ++i)
        {
            sum_current += Current_EnemyCount[i];
        }


        /////////////////////////////////
        if((sum_max - sum_current) < enemies_to_refresh)
        {
            enemies_to_refresh = (sum_max - sum_current); //Do not go over the max enemy count
        }
        last_Refresh = Time.time;
        /////////////////////////////////


        float padding = 0;
        for (int i = 0; i < enemyPool.Count; ++i) //Use the largest padding from all of the enemies
        {
            float temp = enemyPool[i].GetComponent<EnemyTemplateMaster>().SpawnPadding;
            if (temp > padding)
            {
                padding = temp;
            }
        }

        int enemyTypeIter = 0;
        for (int i = 0; i < enemies_to_refresh; i++)
        {
            while(Current_EnemyCount[enemyTypeIter] >= max_enemy[enemyTypeIter]) //Find enemy types that are not at thier cap
            {
                enemyTypeIter += 1;
            }

            float x = (padding * (i % childSqrtRoot)) - childSqrtRoot;
            float z = (padding * (i / childSqrtRoot)) - childSqrtRoot;

            Vector3 modTrans = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

            Transform clone = Instantiate(enemyPool[enemyTypeIter], modTrans, transform.rotation);
            clone.name = clone.name + " " + i; //Not unique

            Children.Add(clone.gameObject);

            clone.GetComponent<EnemyTemplateMaster>().SpawnEnemy(Faction, roam, transform, enemyTypeIter);
            clone.gameObject.SetActive(true);
            AdditionalSetup(i, clone); //After so Awake is called first
        }
        Current_EnemyCount = max_enemy;
    }
}

