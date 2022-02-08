using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EquipTask : QuestTask
{
    [SerializeField] private bool[] Armor = new bool[3];
    [SerializeField] private bool Cons;
    [SerializeField] private bool[] Weapons = new bool[2];

    private Inventory inv;

    protected override void initialize()
    {
        Assert.IsTrue(Armor.Length == 3);
        Assert.IsTrue(Weapons.Length == 2);
        inv = FindObjectOfType<Inventory>();
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!inv)
        {
            return; //Inv won't be set if this is not initalized
        }

        bool complete = true;

        for(int i = 0; i < 3; ++i)
        {
            if (Armor[i] && !inv.ReturnArmor((ArmorType)i))
            {
                complete = false;
            }
        }

        if (Cons && !inv.ReturnConsumable())
        {
            complete = false;
        }

        for (int i = 0; i < 2; ++i)
        {
            if (Weapons[i] && !inv.ReturnWeapon(i))
            {
                complete = false;
            }
        }

        if (complete)
        {
            
            TaskCompletion();
        }
    }
}
