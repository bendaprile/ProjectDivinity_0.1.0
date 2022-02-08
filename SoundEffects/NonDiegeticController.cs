using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NonDiegeticController : MonoBehaviour
{
    [SerializeField] float HardSwitchDuration = 1f;
    [SerializeField] float SwitchDuration = 3f;

    [SerializeField] private AudioSource[] audioSources;

    private float audioLevel;

    [SerializeField] private List<AudioClip> ApocNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> ApocCombatMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> TundraNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> TundraCombatMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> StormNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> StormCombatMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> JungleNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> JungleCombatMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> CityNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> CityCombatMusic = new List<AudioClip>();

    private (List<AudioClip>, List<AudioClip>) ApocMusic;
    private (List<AudioClip>, List<AudioClip>) TundraMusic;
    private (List<AudioClip>, List<AudioClip>) StormMusic;
    private (List<AudioClip>, List<AudioClip>) JungleMusic;
    private (List<AudioClip>, List<AudioClip>) CityMusic;

    private struct MusicData
    {
        public MusicData(int iter_in, float audio_time_in, (List<AudioClip>, List<AudioClip>) Data_in)
        {
            iter = iter_in;
            audio_time = audio_time_in;
            Data = Data_in;
        }

        public int iter;
        public float audio_time;
        public (List<AudioClip>, List<AudioClip>) Data;
    }


    private Stack<MusicData> Playlist = new Stack<MusicData>();

    private CombatChecker CC;

    private float kill_until;
    private bool audio_killed;


    private Zones current_zone = Zones.Wasteland;


    void Awake()
    {
        Assert.IsTrue(audioSources[0].volume == audioSources[1].volume);
        audioLevel = audioSources[0].volume;
        CC = GameObject.Find("Player").GetComponentInChildren<CombatChecker>();

        Playlist.Push(new MusicData());
        audio_killed = true;

        ApocMusic = (ApocNormalMusic, ApocCombatMusic);
        TundraMusic = (TundraNormalMusic, TundraCombatMusic);
        StormMusic = (StormNormalMusic, StormCombatMusic);
        JungleMusic = (JungleNormalMusic, JungleCombatMusic);
        CityMusic = (CityNormalMusic, CityCombatMusic);
    }


    public void ChangeAudioSpecific(List<AudioClip> data)
    {
        MusicData MD_temp = Playlist.Pop();
        MD_temp.audio_time = audioSources[0].time;
        Playlist.Push(MD_temp);

        Playlist.Push(new MusicData(0, 0f, (data, null)));
        kill_until = Time.unscaledTime + HardSwitchDuration;
    }

    public void ChangeAudioGeneral() //TODO MAKE BASED ON LOCATION
    {
        if (Playlist.Count == 1)
        {
            return;
        }
        Playlist.Pop();
        kill_until = Time.unscaledTime + HardSwitchDuration;
    }


    private void Update()
    {
        if(Time.unscaledTime < kill_until)
        {
            if(audioSources[0].volume > 0)
            {
                audioSources[0].volume -= audioLevel * Time.unscaledDeltaTime / HardSwitchDuration;
            }

            if(audioSources[1].volume > 0)
            {
                audioSources[1].volume -= audioLevel * Time.unscaledDeltaTime / HardSwitchDuration;
            }

            audio_killed = true;
            return;
        }


        if (Playlist.Count == 1)
        {
            if (current_zone == Zones.Wasteland && Playlist.Peek().Data != ApocMusic)
            {
                Playlist.Pop();
                Playlist.Push(new MusicData(0, 0f, ApocMusic));
            }
        }


        if(audio_killed) //Happens once
        {
            audio_killed = false;
            audioSources[0].clip = Playlist.Peek().Data.Item1[Playlist.Peek().iter];
            audioSources[0].time = Playlist.Peek().audio_time;
            audioSources[0].Play();

            if (Playlist.Peek().Data.Item2 != null)
            {
                audioSources[1].clip = Playlist.Peek().Data.Item2[Playlist.Peek().iter];
                audioSources[1].time = Playlist.Peek().audio_time;
                audioSources[1].Play();
            }

        }


        if (CC.enemies_nearby && (Playlist.Peek().Data.Item2 != null))
        {
            audioSources[0].volume -= audioLevel * Time.unscaledDeltaTime / SwitchDuration;
            audioSources[1].volume += audioLevel * Time.unscaledDeltaTime / SwitchDuration;
        }
        else
        {
            audioSources[0].volume += audioLevel * Time.unscaledDeltaTime / SwitchDuration;
            audioSources[1].volume -= audioLevel * Time.unscaledDeltaTime / SwitchDuration;
        }





        if (!audioSources[0].isPlaying) //Next track
        {
            MusicData MD_temp = Playlist.Pop();
            MD_temp.iter = (MD_temp.iter + 1) % MD_temp.Data.Item1.Count;
            Playlist.Push(MD_temp);

            audioSources[0].clip = Playlist.Peek().Data.Item1[Playlist.Peek().iter];
            audioSources[0].time = 0;
            audioSources[0].Play();

            if (Playlist.Peek().Data.Item2 != null)
            {
                audioSources[1].clip = Playlist.Peek().Data.Item2[Playlist.Peek().iter];
                audioSources[1].time = 0;
                audioSources[1].Play();
            }
        }
    }

}
