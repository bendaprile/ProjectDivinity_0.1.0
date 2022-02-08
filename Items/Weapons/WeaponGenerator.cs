using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> BasicMelee = new List<GameObject>();
    [SerializeField] private List<GameObject> AdvancedMelee = new List<GameObject>();

    [SerializeField] private List<GameObject> BasicRanged = new List<GameObject>();
    [SerializeField] private List<GameObject> AdvancedRanged = new List<GameObject>();

    [Range(0.0f, 99.0f)][SerializeField] private float Rarity = 0; //0 to 99

    

    private float[] QualityChances = new float[] { .1f, .25f, .75f, .9f, 1f, 2f }; //Basically impossible to get a legendary here (have to get exactly 1)

    // Start is called before the first frame update
    void Start()
    {
        GameObject ItemChosen = null;


        float typeNum = Random.value;

        float ADVNum = ((Rarity / 200) + Random.value) / 2; //Leave these independent
        float QualityNum = ((Rarity / 100) + Random.value) / 2; //Leave these independent

        if (typeNum > .5)
        {
            if (ADVNum > .5f) //0% to 50%
            {
                int chosen = Random.Range(0, AdvancedMelee.Count);
                ItemChosen = AdvancedMelee[chosen];
            }
            else
            {
                int chosen = Random.Range(0, BasicMelee.Count);
                ItemChosen = BasicMelee[chosen];
            }
        }
        else
        {
            if (ADVNum > .5f) //0% to 50%
            {
                int chosen = Random.Range(0, AdvancedRanged.Count);
                ItemChosen = AdvancedRanged[chosen];
            }
            else
            {
                int chosen = Random.Range(0, BasicRanged.Count);
                ItemChosen = BasicRanged[chosen];
            }
        }
        GameObject TempItem = Instantiate(ItemChosen, transform.parent);

        ItemQuality itemClass = ItemQuality.Damaged;
        for(int i = 0; i < QualityChances.Length; i++)
        {
            if(QualityNum < QualityChances[i])
            {
                itemClass = (ItemQuality)i;
                break;
            }
        }

        TempItem.GetComponent<Weapon>().InitializeValues(itemClass);
        TempItem.SetActive(false);

        Destroy(gameObject);
    }
}
