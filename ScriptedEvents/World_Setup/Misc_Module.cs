using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Misc_Module : Virtual_Module
{
    [SerializeField] protected List<AudioClip> Playlist_to_play = new List<AudioClip>();
    [SerializeField] protected bool revert_music_to_general = false;

    [Space(35)]

    [SerializeField] protected List<Transform> Transforms_to_move = new List<Transform>();
    [SerializeField] protected List<Transform> Dest_transforms = new List<Transform>();

    private NonDiegeticController NDC;


    public override void Setup()
    {
        Assert.IsTrue(Transforms_to_move.Count == Dest_transforms.Count);
        NDC = FindObjectOfType<NonDiegeticController>();
    }


    public override void Run()
    {
        if (Playlist_to_play.Count != 0)
        {
            NDC.ChangeAudioSpecific(Playlist_to_play);
        }
        else if (revert_music_to_general)
        {
            NDC.ChangeAudioGeneral();
        }


        for (int i = 0; i < Transforms_to_move.Count; ++i)
        {
            Transforms_to_move[i].position = Dest_transforms[i].position;
        }
    }
}
