using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class HumanFootstepsScriptableObject : ScriptableObject
{
    // Concrete Sounds
    public AudioClip[] walkConcrete;
    public AudioClip[] runConcrete;

    // Wood Sounds
    public AudioClip[] walkWood;
    public AudioClip[] runWood;

    // Metal Sounds
    public AudioClip[] walkMetal;
    public AudioClip[] runMetal;

    // Sand Sounds
    public AudioClip[] walkSand;
    public AudioClip[] runSand;

    // Gravel Sounds
    public AudioClip[] walkGravel;
    public AudioClip[] runGravel;

    // Snow Sounds
    public AudioClip[] walkSnow;
    public AudioClip[] runSnow;

    // Grass Sounds
    public AudioClip[] walkGrass;
    public AudioClip[] runGrass;
}
