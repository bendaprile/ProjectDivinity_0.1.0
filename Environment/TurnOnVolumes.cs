using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnVolumes : MonoBehaviour
{
    [SerializeField] GameObject Volumes;

    void Awake()
    {
        Volumes.SetActive(true);
    }
}
