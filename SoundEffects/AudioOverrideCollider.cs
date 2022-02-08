using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOverrideCollider : MonoBehaviour
{
    [SerializeField] private List<AudioClip> Clips;
    private SphereCollider col;
    private Transform player;
    private NonDiegeticController NDC;

    private bool isPlaying = false;


    private void Start()
    {
        col = GetComponent<SphereCollider>();
        player = GameObject.Find("Player").transform;
        NDC = FindObjectOfType<NonDiegeticController>();
    }


    private void MusicLogic()
    {
        float dist = (player.position - transform.position).magnitude;

        if (isPlaying)
        {
            if(dist > col.radius)
            {
                isPlaying = false;
                NDC.ChangeAudioGeneral();
            }
        }
        else
        {
            if (dist < col.radius)
            {
                isPlaying = true;
                NDC.ChangeAudioSpecific(Clips);
            }
        }
    }


    private void FixedUpdate()
    {
        MusicLogic();
    }

    private void OnDisable() //Fixed update won't happen if the player teleports and the terrain is disabled
    {
        if (isPlaying)
        {
            NDC.ChangeAudioGeneral();
        }
        isPlaying = false;
    }
}
