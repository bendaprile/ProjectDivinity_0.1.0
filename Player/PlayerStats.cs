using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerStats : MonoBehaviour
{
    [SerializeField] private string playerName = "No Name Set";
    [SerializeField] private Transform EXPUI = null;
    private Image EXPBar;
    private Image EXPBarGhost;

    private AbilitiesController abilitiesController;
    private GameObject Player;
    private Health PlayerHealth;
    private Energy PlayerEnergy;
    private EventQueue eventQueue;

    //////////////////////////////////////
    private int level;
    private int current_exp;
    private int[] ImplantPoints = new int[3];
    private float FinishedBarDuration = 0;
    private bool LevelUpQued = false;


    public void AddEXP(int exp_in)
    {
        abilitiesController.DistributeXP(exp_in);
        current_exp += (exp_in);
        FinishedBarDuration = 2;
        EXPUI.gameObject.SetActive(true);
        EXPUI.Find("EXPNum").GetComponent<TextMeshProUGUI>().text = "+" + exp_in.ToString();
        EXPUI.Find("CharacterLevel").GetComponentInChildren<TextMeshProUGUI>().text = "LEVEL " + level;
    }

    private void UpdateDisplayXP()
    {
        EXPBarGhost.fillAmount = Mathf.Min((float)current_exp / (level * 1000), 1f);

        if (FinishedBarDuration > 0)
        {
            if (EXPBar.fillAmount < EXPBarGhost.fillAmount)
            {
                EXPBar.fillAmount += 0.125f * Time.deltaTime;
            }
            else
            {
                EXPBar.fillAmount = EXPBarGhost.fillAmount;
                FinishedBarDuration -= Time.deltaTime;
                if (current_exp >= (level * 1000) && !LevelUpQued)
                {
                    QueueLevelUp();
                }
            }
        }
        else if (current_exp < (level * 1000)) //Don't disable when the player can level up
        {
            EXPUI.gameObject.SetActive(false);
        }
        else if (LevelUpQued)
        {
            EXPUI.Find("EXPNum").GetComponent<TextMeshProUGUI>().text = "LEVEL UP!";
        }
    }

    public int returnLevel()
    {
        return level;
    }

    public string getPlayerName()
    {
        return playerName;
    }

    public int returnImplantPoints(AptitudeEnum AE)
    {
        return ImplantPoints[(int)AE];
    }

    public void modifyImplantPoints(AptitudeEnum AE, int change)
    {
        ImplantPoints[(int)AE] += change;
    }

    private void QueueLevelUp()
    {
        LevelUpQued = true;

        /////
        EventData tempEvent = new EventData();
        tempEvent.Setup(EventTypeEnum.LevelUp, "");
        eventQueue.AddEvent(tempEvent);
        /////
    }

    public int LevelUp()
    {
        int level_ups = 0;
        while(current_exp >= level * 1000) //Handles the edge case of multiple levelups
        {
            level_ups += 1;
            LevelUp_single();
        }

        LevelUpQued = false;
        EXPBar.fillAmount = 0f;
        EXPBarGhost.fillAmount = Mathf.Min((float)current_exp / (level * 1000), 1f);
        FinishedBarDuration = 2;
        return level_ups;
    }

    private void LevelUp_single()
    {
        current_exp -= level * 1000;

        for (int i = 0; i < 3; ++i)
        {
            modifyImplantPoints((AptitudeEnum)i, AptitudeStorage[i]);
        }
        level += 1;
        EXPUI.Find("EXPNum").GetComponent<TextMeshProUGUI>().text = "";
    }


    //////////////////////////////////////



    //////////////////////////////////////
    private int[] AptitudeStorage = new int[3];

    public void SetAptitude(AptitudeEnum Aptitude, int value)
    {
        AptitudeStorage[(int)Aptitude] = value;
    }

    public int ReturnAptitude(AptitudeEnum Aptitude)
    {
        return AptitudeStorage[(int)Aptitude];
    }
    //////////////////////////////////////



    //////////////////////////////////////
    private float[] FinalAttributesStorage = new float[STARTUP_DECLARATIONS.Number_of_Attributes];
    public List<(string, float)>[] AttributesAdditiveEffects = new List<(string, float)>[STARTUP_DECLARATIONS.Number_of_Attributes];
    public List<(string, float)>[] AttributesMultiplicativeEffects = new List<(string, float)>[STARTUP_DECLARATIONS.Number_of_Attributes];

    private void UpdateAttributes_Base()
    {
        AddAttributeEffect(AttributesEnum.max_health, "Base", true, 100);
        AddAttributeEffect(AttributesEnum.health_regen, "Base", true, .1f);

        AddAttributeEffect(AttributesEnum.max_energy, "Base", true, 400);
        AddAttributeEffect(AttributesEnum.max_overheat, "Base", true, 80);

        AddAttributeEffect(AttributesEnum.energy_regen, "Base", true, 15f);
        AddAttributeEffect(AttributesEnum.cooldown_rate, "Base", true, 0.20f);

        AddAttributeEffect(AttributesEnum.vigor_mult, "Base", true, 1f);
        AddAttributeEffect(AttributesEnum.cerebral_mult, "Base", true, 1f);
        AddAttributeEffect(AttributesEnum.finesse_mult, "Base", true, 1f);
    }

    private void UpdateAttributes_skill()
    {
        AddAttributeEffect(AttributesEnum.health_regen, STARTUP_DECLARATIONS.SkillEnumReverse[(int)SkillsEnum.Survival], true, (float)SkillsStorage[(int)SkillsEnum.Survival] / 40f);
    }

    public float ReturnAttribute(AttributesEnum AttributeName)
    {
        return FinalAttributesStorage[(int)AttributeName];
    }

    public void AddAttributeEffect(AttributesEnum AttributeName, string EffectName, bool isAdd, float value) //Overwrites effect with the same name
    {
        if (isAdd)
        {
            for(int location = 0; location < AttributesAdditiveEffects[(int)AttributeName].Count; ++location)
            {
                if(AttributesAdditiveEffects[(int)AttributeName][location].Item1 == EffectName)
                {
                    AttributesAdditiveEffects[(int)AttributeName][location] = (EffectName, value);
                    RecalculateAttribute((int)AttributeName);
                    return;
                }
            }
            AttributesAdditiveEffects[(int)AttributeName].Add((EffectName, value));
        }
        else
        {
            for (int location = 0; location < AttributesMultiplicativeEffects[(int)AttributeName].Count; ++location)
            {
                if (AttributesMultiplicativeEffects[(int)AttributeName][location].Item1 == EffectName)
                {
                    AttributesMultiplicativeEffects[(int)AttributeName][location] = (EffectName, value);
                    RecalculateAttribute((int)AttributeName);
                    return;
                }
            }
            AttributesMultiplicativeEffects[(int)AttributeName].Add((EffectName, value));
        }
        RecalculateAttribute((int)AttributeName);
    }

    public void RemoveAttributeEffect(AttributesEnum AttributeName, string EffectName, bool isAdd)
    {
        if (isAdd)
        {
            int len = AttributesAdditiveEffects[(int)AttributeName].Count;
            for (int i = 0; i < len; i++)
            {
                if (AttributesAdditiveEffects[(int)AttributeName][i].Item1 == EffectName)
                {
                    AttributesAdditiveEffects[(int)AttributeName].RemoveAt(i);
                    break;
                }
            }
        }
        else
        {
            int len = AttributesMultiplicativeEffects[(int)AttributeName].Count;
            for (int i = 0; i < len; i++)
            {
                if (AttributesMultiplicativeEffects[(int)AttributeName][i].Item1 == EffectName)
                {
                    AttributesMultiplicativeEffects[(int)AttributeName].RemoveAt(i);
                    break;
                }
            }
        }
        RecalculateAttribute((int)AttributeName);
    }

    public List<(string, float)> ReturnAttributeEffects(AttributesEnum AttributeName, bool isAdd)
    {
        if (isAdd)
        {
            return AttributesAdditiveEffects[(int)AttributeName];
        }
        else
        {
            return AttributesMultiplicativeEffects[(int)AttributeName];
        }
    }

    private void RecalculateAttribute(int AttributeLoc)
    {
        float temp = 0;

        foreach ((string, float) addTemp in AttributesAdditiveEffects[AttributeLoc])
        {
            temp += addTemp.Item2;
        }

        float MultModifier = 1;
        foreach ((string, float) multTemp in AttributesMultiplicativeEffects[AttributeLoc])
        {
            MultModifier += multTemp.Item2;
        }

        temp *= MultModifier;

        FinalAttributesStorage[AttributeLoc] = temp;
        UpdateExternalAttributes();
    }

    private void UpdateExternalAttributes()
    {
        PlayerHealth.modify_maxHealth(ReturnAttribute(AttributesEnum.max_health));
        PlayerHealth.modify_healthRegen(ReturnAttribute(AttributesEnum.health_regen));
        PlayerHealth.modify_Armor((int)ReturnAttribute(AttributesEnum.armor));
        PlayerEnergy.modify_maxEnergy(ReturnAttribute(AttributesEnum.max_energy));
        PlayerEnergy.modify_OverheatCapacity(ReturnAttribute(AttributesEnum.max_overheat));
        PlayerEnergy.modify_energyRegen(ReturnAttribute(AttributesEnum.energy_regen));
        PlayerEnergy.modify_CooldownRate(ReturnAttribute(AttributesEnum.cooldown_rate));
    }
    //////////////////////////////////////



    //////////////////////////////////////
    private int[] SkillsStorage = new int[7];

    public void ModifySkill(SkillsEnum SkillName, int value)
    {
        SkillsStorage[(int)SkillName] += value;
        UpdateAttributes_skill();
    }

    public int ReturnSkill(SkillsEnum SkillName)
    {
        return SkillsStorage[(int)SkillName];
    }
    //////////////////////////////////////


    private void Start()
    {
        Player = GameObject.Find("Player");
        abilitiesController = Player.GetComponentInChildren<AbilitiesController>();
        PlayerHealth = Player.GetComponentInChildren<Health>();
        PlayerEnergy = Player.GetComponent<Energy>();
        eventQueue = GameObject.Find("EventDisplay").GetComponent<EventQueue>();

        EXPBarGhost = EXPUI.Find("EXPBarGhost").GetComponent<Image>();
        EXPBar = EXPUI.Find("EXPBar").GetComponent<Image>();

        for (int i = 0; i < STARTUP_DECLARATIONS.Number_of_Attributes; i++)
        {
            AttributesAdditiveEffects[i] = new List<(string, float)>();
            AttributesMultiplicativeEffects[i] = new List<(string, float)>();
        }

        level = 1;
        current_exp = 0;
        EXPBar.fillAmount = 0;
        SetAptitude(AptitudeEnum.Vigor, 4);
        SetAptitude(AptitudeEnum.Cerebral, 4);
        SetAptitude(AptitudeEnum.Finesse, 4);
        UpdateAttributes_Base();

        EXPUI.Find("CharacterLevel").Find("Text").GetComponent<TextMeshProUGUI>().text = "LEVEL " + level.ToString();
        /////////////////////////////////////
    }


    public float ReturnDamageMult(float Damage_in, DamageSource damageSource)
    {
        if(damageSource == DamageSource.VigorBased) //Do nothing in "None" damageSource
        {
            Damage_in *= ReturnAttribute(AttributesEnum.vigor_mult);
        }
        else if(damageSource == DamageSource.CerebralBased)
        {
            Damage_in *= ReturnAttribute(AttributesEnum.cerebral_mult);
        }
        else if(damageSource == DamageSource.FinesseBased)
        {
            Damage_in *= ReturnAttribute(AttributesEnum.finesse_mult);
        }

        return Damage_in;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplayXP();
    }

    public bool isLevelUpQueued()
    {
        return LevelUpQued;
    }
}
