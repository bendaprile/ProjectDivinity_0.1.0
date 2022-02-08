using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    [SerializeField] private GameObject tracker;
    [SerializeField] private Transform trackerParent;

    private List<GameObject> trackers = new List<GameObject>();


    private CombatChecker CC;
    Transform player;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        CC = player.GetComponentInChildren<CombatChecker>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y - .9f, player.position.z);



        for (int i = trackers.Count; i < CC.EnemyOnlyList.Count; ++i) //Add trackers
        {
            GameObject temp = Instantiate(tracker, trackerParent);
            trackers.Add(temp);
        }


        for (int i = 0; i < CC.EnemyOnlyList.Count; ++i) //Move trackers
        {
            trackers[i].SetActive(true);
            Vector3 pos = CC.EnemyOnlyList[i].transform.position - transform.position;
            Vector2 modPos = new Vector2(pos.x, pos.z);

            float mag = 0.2f + (0.8f * (1 - (modPos.magnitude / 100)));

            if (modPos.magnitude > 3)
            {
                modPos.Normalize();
                modPos *= 3;
            }

            trackers[i].transform.localPosition = modPos;

            float rot = Mathf.Atan2(modPos.y, modPos.x) * Mathf.Rad2Deg + 180f;
            trackers[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rot));

            trackers[i].transform.localScale = new Vector3(mag, mag, mag);
        }

        for (int i = CC.EnemyOnlyList.Count; i < trackers.Count; ++i) //Disable
        {
            trackers[i].SetActive(false);
        }
    }
}
