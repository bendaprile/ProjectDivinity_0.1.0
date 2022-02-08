using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public class EnemyArmor : MonoBehaviour
{
    [SerializeField] private GameObject Head;
    [SerializeField] private GameObject Chest;
    [SerializeField] private GameObject Legs;
    private DynamicCharacterAvatar enemyAvatar;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        enemyAvatar = transform.parent.GetComponentInChildren<DynamicCharacterAvatar>();
        UpdateStats();
        UpdateWardrobe();
    }

    public void AttachArmor(GameObject h, GameObject c, GameObject l)
    {
        Head = h;
        Chest = c;
        Legs = l;
        UpdateStats();
        UpdateWardrobe();
    }

    private void UpdateWardrobe()
    {
        if (Head) {
            enemyAvatar.SetSlot(Head.GetComponent<Armor>().GetWardrobePiece().Item1);
        }

        if (Chest) {
            enemyAvatar.SetSlot(Chest.GetComponent<Armor>().GetWardrobePiece().Item1);
            if (Chest.GetComponent<Armor>().GetWardrobePiece().Item2) { enemyAvatar.SetSlot(Chest.GetComponent<Armor>().GetWardrobePiece().Item2); }

        }

        if (Legs) {
            enemyAvatar.SetSlot(Legs.GetComponent<Armor>().GetWardrobePiece().Item1);
            if (Legs.GetComponent<Armor>().GetWardrobePiece().Item2) { enemyAvatar.SetSlot(Legs.GetComponent<Armor>().GetWardrobePiece().Item2); }

        }

        if (enemyAvatar) { enemyAvatar.BuildCharacter(); }
    }

    private void UpdateStats()
    {
        int Armor = 0;
        int Plating = 0;

        if (Head)
        {
            Armor += (int)Head.GetComponent<Armor>().returnArmor();
            Plating += (int)Head.GetComponent<Armor>().returnPlating();
        }

        if (Chest)
        {
            Armor += (int)Chest.GetComponent<Armor>().returnArmor();
            Plating += (int)Chest.GetComponent<Armor>().returnPlating();
        }

        if (Legs)
        {
            Armor += (int)Legs.GetComponent<Armor>().returnArmor();
            Plating += (int)Legs.GetComponent<Armor>().returnPlating();
        }

        health.modify_Armor(Armor);
        health.modify_Plating(Plating);
    }
}
