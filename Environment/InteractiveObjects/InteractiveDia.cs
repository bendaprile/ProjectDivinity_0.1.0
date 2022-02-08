using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class InteractiveDia : InteractiveThing
{
    [SerializeField] private DiaRoot DR = null;

    private bool in_combat;

    private Transform overhead_text;
    private TextMeshPro overHead_t_mesh;

    protected override void Awake()
    {
        Assert.IsFalse(LOCK_UNTIL_QUEST); //DO NOT USE (No reason not to, but not needed... yet..)

        overhead_text = transform.Find("Text");
        overHead_t_mesh = GetComponentInChildren<TextMeshPro>();

        base.Awake();

        SetCombat(false);
    }

    public void SetCombat(bool set)
    {
        in_combat = set;
        overhead_text.gameObject.SetActive(set);
        overHead_t_mesh.text = DR.ReturnNPC_name();
    }

    public void CombatText(string str)
    {
        StartCoroutine(DisplayOverhead(str));
    }

    private IEnumerator DisplayOverhead(string str)
    {
        overHead_t_mesh.color = STARTUP_DECLARATIONS.goldColor;
        overHead_t_mesh.text = str;

        float timer = 5;
        while(timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        overHead_t_mesh.color = Color.white;
        overHead_t_mesh.text = DR.ReturnNPC_name();
    }

    protected override void ActivateLogic()
    {
        if (!in_combat)
        {
            miscDisplay.enableDisplay(DR.ReturnNPC_name(), "(F) Talk");
            if (Input.GetKeyDown(KeyCode.F))
            {
                ForcedActivate();
            }
        }
    }

    public void ForcedActivate()
    {
        QuestActiveLogic();
        Transform StartingTrans = DR.ReturnStarting();
        DiaOverhead diaOver = StartingTrans.GetComponent<DiaOverhead>();
        if (diaOver)
        {
            miscDisplay.enableDisplay(DR.ReturnNPC_name(), diaOver.return_line(), dur: 4f, LOCK_in: true);
        }
        else
        {
            UIControl.DialogueMenuBool(DR.transform);
        }
    }

    protected override void AdditionalUpdateLogic()
    {
        if (in_combat)
        {
            rotate(overhead_text);
        }
    }

    protected override bool HasTask()
    {
        return DR.dia_quest_objective_count > 0;
    }

    protected override bool StartsQuest()
    {
        return DR.has_quest;
    }

    protected override bool IsMerchant()
    {
        return DR.is_merchant;
    }
}
