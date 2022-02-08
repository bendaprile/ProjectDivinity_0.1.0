using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private Transform ammoPrefab = null;
    private Image[] ammoArray;
    private int current_iter;
    private int AmmoCount;

    private Color originalColor;
    private Color depletedAmmo = new Color(0f, 0f, 0f, .4f);

    private void Start()
    {
        originalColor = ammoPrefab.GetComponent<Image>().color;
    }

    public void Setup(int AmmoCount_in, int AmmoLeft)
    {
        AmmoCount = AmmoCount_in;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        ammoArray = new Image[AmmoCount];
        for (int i = 0; i < AmmoCount; i++)
        {
            ammoArray[i] = Instantiate(ammoPrefab, transform).GetComponent<Image>();
        }
        current_iter = 0;

        for(int i = 0; i < (AmmoCount_in - AmmoLeft); ++i)
        {
            UseAmmo();
        }
    }

    public void UseAmmo()
    {
        ammoArray[current_iter].color = depletedAmmo;
        current_iter += 1;
    }

    public void Reload()
    {
        for (int i = 0; i < AmmoCount; i++)
        {
            ammoArray[i].color = originalColor;
        }
        current_iter = 0;
    }
}
