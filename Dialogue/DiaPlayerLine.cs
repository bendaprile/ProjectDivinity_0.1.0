using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class DiaPlayerLine : DiaMaster
{
    [SerializeField] [TextArea(3, 10)] private string Line = "NotSet";

    [SerializeField] public GameObject FlagRef_req;
    [SerializeField] public bool FlagRefVal_req;

    [SerializeField] public SkillCheckDifficulty SkillLevelReqEnum = SkillCheckDifficulty._None_;
    [SerializeField] public SkillsEnum SkillCheck = SkillsEnum.Speech;

    [SerializeField] public GameObject Merchant;



    /// Set //////////////////////////////////////////
    [SerializeField] private Transform NewStartingLine = null;
    [SerializeField] private List<GameObject> DisableList = null; //If used for Dia can only disable player options
    [SerializeField] private List<GameObject> EnableList = null; //If used for Dia can only disable player options
    /// Set //////////////////////////////////////////

    private Zone_Flags ZF;
    private PlayerStats PS;

    public string return_line()
    {
        return Line;
    }

    public bool Check_Accessible(TextMeshProUGUI textRef) //Can use with textRef or without
    {
        PS = GameObject.Find("Player").GetComponent<PlayerStats>();
        ZF = GameObject.Find("Master Object").GetComponentInChildren<Zone_Flags>();

        string viewable_text = "";
        bool Accessible = true;

        if (SkillLevelReqEnum != SkillCheckDifficulty._None_)
        {
            int charskill = PS.ReturnSkill(SkillCheck);
            if (PS.ReturnSkill(SkillCheck) >= STARTUP_DECLARATIONS.ReturnSkillCheck(PS.returnLevel(), SkillLevelReqEnum))
            {
                if (textRef)
                {
                    textRef.color = STARTUP_DECLARATIONS.checkSuccessColor;
                }
            }
            else
            {
                if (textRef)
                {
                    textRef.color = STARTUP_DECLARATIONS.checkFailColor;
                }
                Accessible = false;
            }

            viewable_text += "[" + STARTUP_DECLARATIONS.SkillEnumReverse[(int)SkillCheck] + " (" + charskill + "/" + STARTUP_DECLARATIONS.ReturnSkillCheck(PS.returnLevel(), SkillLevelReqEnum) + ")] ";
        }


        if (FlagRef_req && ZF.CheckFlag(FlagRef_req.name) != FlagRefVal_req) //DO NOT SHOW IF FLAG ISN'T CORRECT
        {
            viewable_text = "Option from another universe";
            if (textRef)
            {
                textRef.color = Color.grey;
            }
            Accessible = false;
        }
        else if(textRef)
        {
            if (textRef.color != STARTUP_DECLARATIONS.checkFailColor)
            {
                viewable_text += return_line();
            }
        }


        if (MakeHostile)
        {
            viewable_text += " [Attack]";
        }
        else if (Merchant != null)
        {
            Assert.IsNull(Dest, "Cannot have both a merchant and a dest");
            viewable_text += " [Trade]";
        }
        else if (Dest == null)
        {
            viewable_text += " [Leave Conversation]";
        }





        if (textRef)
        {
            textRef.text = viewable_text;
        }

        return Accessible;
    }


    public bool return_has_check()
    {
        return SkillLevelReqEnum != SkillCheckDifficulty._None_;
    }

    public bool return_flag_fail()
    {
        return FlagRef_req && ZF.CheckFlag(FlagRef_req.name) != FlagRefVal_req;
    }

    public Transform return_new_start()
    {
        return NewStartingLine;
    }

    public Transform return_dest(bool real = true) //This means that it is clicked
    {
        if (real)
        {
            foreach (GameObject temp in DisableList)
            {
                temp.SetActive(false);

                DiaRoot DR = temp.GetComponentInParent<DiaRoot>();
                if (DR != null)
                {
                    DR.check_for_quest_merchant();
                }
            }

            foreach (GameObject temp in EnableList)
            {
                temp.SetActive(true);

                DiaRoot DR = temp.GetComponentInParent<DiaRoot>();
                if (DR != null)
                {
                    DR.check_for_quest_merchant();
                }
            }
        }

        return Dest;
    }
}
