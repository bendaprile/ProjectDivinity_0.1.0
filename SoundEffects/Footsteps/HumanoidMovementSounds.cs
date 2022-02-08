using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidMovementSounds : MonoBehaviour
{

    [SerializeField] private HumanFootstepsScriptableObject sounds;
    [SerializeField] private float jogThreshold = 6.5f; //TODO Grab these from somewhere
    [SerializeField] private float runThreshold = 9f; //TODO Grab these from somewhere

    private AudioSource audioSource;
    private AudioClip[] currentSounds;
    private TerrainDetector terrainDetector;
    private Terrain currentTerrain;

    private float movementSpeed = 0f;
    private bool playSound = false;

    private int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        terrainDetector = FindObjectOfType<TerrainDetector>();
        layerMask = LayerMask.GetMask("Terrain");
    }

    private void FixedUpdate()
    {
        movementSpeed = GetComponentInParent<Rigidbody>().velocity.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, layerMask))
        {
            playSound = true;
            if ((!currentTerrain && hit.transform.GetComponent<Terrain>()) || (hit.transform.GetComponent<Terrain>() && currentTerrain.name != hit.transform.GetComponent<Terrain>().name))
            {
                currentTerrain = hit.transform.GetComponent<Terrain>();
                terrainDetector = currentTerrain.GetComponent<TerrainDetector>();
            }

            if (hit.transform.tag == "Concrete")
            {
                currentSounds = movementSpeed >= jogThreshold ? sounds.runConcrete : sounds.walkConcrete;
            }
            else if (hit.transform.tag == "Wood")
            {
                currentSounds = movementSpeed >= jogThreshold ? sounds.runWood : sounds.walkWood;
            }
            else if (hit.transform.tag == "Metal")
            {
                currentSounds = movementSpeed >= jogThreshold ? sounds.runMetal : sounds.walkMetal;
            }
            else if (hit.transform.tag == "Terrain")
            {
                //Debug.Log(hit.transform.gameObject.name);
                int textureIndex = terrainDetector.GetDominantTextureIndexAt(transform.position);
                switch (textureIndex)
                {
                    case 0: // sand texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runSand : sounds.walkSand;
                        break;
                    case 1: // gravel texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runGravel : sounds.walkGravel;
                        break;
                    case 2: // moss texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runGrass : sounds.walkGrass;
                        break;
                    case 3: // grass texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runGrass : sounds.walkGrass;
                        break;
                    case 4: // rock texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runConcrete : sounds.walkConcrete;
                        break;
                    case 5: // charcoal texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runConcrete : sounds.walkConcrete;
                        break;
                    case 6: // snow texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runSnow : sounds.walkSnow;
                        break;
                    case 7: // dirt texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runGrass : sounds.walkGrass;
                        break;
                    case 8: // forest path texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runSand : sounds.walkSand;
                        break;
                    case 9:
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runConcrete : sounds.walkConcrete;
                        break;
                    default:
                        Debug.LogWarning("Please add footsteps sound effect for this ground type: " + textureIndex);
                        break;
                }
            }
        }
        else
        {
            playSound = false;
        }
    }


    //TODO: Get rid of this code when we know we don't want any of it anymore
    /*
    private void OnTriggerStay(Collider other)
    {
        movementSpeed = GetComponentInParent<Rigidbody>().velocity.magnitude;

        if ((!currentTerrain && other.GetComponent<Terrain>()) || (other.GetComponent<Terrain>() && currentTerrain.name != other.GetComponent<Terrain>().name))
        {
            currentTerrain = other.GetComponent<Terrain>();
            terrainDetector = currentTerrain.GetComponent<TerrainDetector>();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Debug.Log(("Terrain", Time.time));
            if (other.tag == "Concrete")
            {
                currentSounds = movementSpeed >= jogThreshold ? sounds.runConcrete : sounds.walkConcrete;
            }
            else if (other.tag == "Wood")
            {
                currentSounds = movementSpeed >= jogThreshold ? sounds.runWood : sounds.walkWood;
            }
            else if (other.tag == "Metal")
            {
                currentSounds = movementSpeed >= jogThreshold ? sounds.runMetal : sounds.walkMetal;
            }
            else if (other.tag == "Terrain")
            {
                int textureIndex = terrainDetector.GetDominantTextureIndexAt(transform.position);

                switch (textureIndex)
                {
                    case 0: // sand texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runSand : sounds.walkSand;
                        break;
                    case 1: // gravel texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runGravel : sounds.walkGravel;
                        break;
                    case 2: // moss texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runGrass : sounds.walkGrass;
                        break;
                    case 3: //grass texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runGrass : sounds.walkGrass;
                        break;
                    case 4: //rock texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runConcrete : sounds.walkConcrete;
                        break;
                    case 5: //charcoal texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runConcrete : sounds.walkConcrete;
                        break;
                    case 6: //snow texture
                        currentSounds = movementSpeed >= jogThreshold ? sounds.runSnow : sounds.walkSnow;
                        break;
                }
            }
        }
    }

    private void Update()
    {
        if (IsNotColliding())
        {
            playSound = false;
        }
        else
        {
            playSound = true;
        }

        if (counter < 5f)
        {
            counter += Time.unscaledDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 && counter >= 5f)
        {
            collisionCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 && counter >= 5f)
        {
            collisionCount--;
        }
    }

    public bool IsNotColliding()
    {
        return collisionCount == 0;
    }
    */

    private void WalkStep()
    {
        if (movementSpeed < jogThreshold && playSound)
        {
            AudioClip clip = GetRandomClip(currentSounds);
            audioSource.PlayOneShot(clip);
        }
    }

    private void JogStep()
    {
        if (movementSpeed >= jogThreshold && movementSpeed < runThreshold && playSound)
        {
            AudioClip clip = GetRandomClip(currentSounds);
            audioSource.PlayOneShot(clip);
        }
    }

    private void RunStep()
    {
        if (movementSpeed >= runThreshold && playSound)
        {
            AudioClip clip = GetRandomClip(currentSounds);
            audioSource.PlayOneShot(clip);
        }
    }

    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
