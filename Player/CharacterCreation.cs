using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA.CharacterSystem;
using UMA;
using UnityEngine.UI;
using System.IO;

public class CharacterCreation : MonoBehaviour
{
    [SerializeField] private List<string> maleHairStyles = new List<string>();
    [SerializeField] private List<string> femaleHairStyles = new List<string>();
    [SerializeField] private List<string> facialHairStyles = new List<string>();
    [SerializeField] private GameObject maleHairSelector = null;
    [SerializeField] private GameObject femaleHairSelector = null;
    [SerializeField] private GameObject facialHairSelector = null;
    [SerializeField] private DynamicCharacterAvatar avatar = null;

    [Header("Body Sliders")]
    [SerializeField] private Slider heightSlider = null;
    [SerializeField] private Slider upperWeightSlider = null;
    [SerializeField] private Slider lowerWeightSlider = null;
    [SerializeField] private Slider upperMuscleSlider = null;
    [SerializeField] private Slider lowerMuscleSlider = null;
    [SerializeField] private Slider breastSlider = null;
    [SerializeField] private Slider feetSlider = null;
    [SerializeField] private Slider armSlider = null;
    [SerializeField] private Slider forearmSlider = null;
    [SerializeField] private Slider gluteusSlider = null;
    [SerializeField] private Slider legSeparationSlider = null;
    [SerializeField] private Slider legsSizeSlider = null;
    [SerializeField] private Slider bellySlider = null;
    [SerializeField] private Slider waistSlider = null;

    [Header("Head Sliders")]
    [SerializeField] private Slider headSizeSlider = null;
    [SerializeField] private Slider headWidthSlider = null;
    [SerializeField] private Slider neckThicknessSlider = null;
    [SerializeField] private Slider earsPosSlider = null;
    [SerializeField] private Slider earsRotSlider = null;
    [SerializeField] private Slider earsSizeSlider = null;
    [SerializeField] private Slider foreheadPosSlider = null;
    [SerializeField] private Slider foreheadSizeSlider = null;

    [Header("Face Sliders")]
    //eyes
    [SerializeField] private Slider eyeRotSlider = null;
    [SerializeField] private Slider eyeSizeSlider = null;
    [SerializeField] private Slider eyeSpacingSlider = null;

    //cheeks
    [SerializeField] private Slider cheekPosSlider = null;
    [SerializeField] private Slider cheekSizeSlider = null;
    [SerializeField] private Slider lowCheekPosSlider = null;
    [SerializeField] private Slider lowCheekPronouncedSlider = null;

    //nose
    [SerializeField] private Slider noseCurveSlider = null;
    [SerializeField] private Slider noseFlattenSlider = null;
    [SerializeField] private Slider noseInclinationSlider = null;
    [SerializeField] private Slider nosePosSlider = null;
    [SerializeField] private Slider nosePronouncedSlider = null;
    [SerializeField] private Slider noseSizeSlider = null;
    [SerializeField] private Slider noseWidthSlider = null;

    //mouth
    [SerializeField] private Slider lipsSizeSlider = null;
    [SerializeField] private Slider mouthSizeSlider = null;

    //jaw & chin
    [SerializeField] private Slider mandibleSizeSlider = null;
    [SerializeField] private Slider jawPosSlider = null;
    [SerializeField] private Slider jawSizeSlider = null;
    [SerializeField] private Slider chinPosSlider = null;
    [SerializeField] private Slider chinPronouncedSlider = null;
    [SerializeField] private Slider chinSizeSlider = null;

    private int maleHairIndex = 0;
    private int femaleHairIndex = 0;
    private int facialHairIndex = 0;
    
    private Color32[] skinColors = new Color32[10];
    private int skinIndex = 0;

    private Color32[] hairColors = new Color32[12];
    private int hairColorIndex = 0;

    private Color32[] eyeColors = new Color32[9];
    private int eyeColorIndex = 0;

    private Dictionary<string, DnaSetter> dna;
    private string dnaString = "";
    private string myRecipe;

    private void Start()
    {
        skinColors.SetValue(new Color32(255, 255, 255, 255), 0);
        skinColors.SetValue(new Color32(229, 223, 204, 255), 1);
        skinColors.SetValue(new Color32(209, 195, 159, 255), 2);
        skinColors.SetValue(new Color32(202, 176, 117, 255), 3);
        skinColors.SetValue(new Color32(176, 144, 106, 255), 4);
        skinColors.SetValue(new Color32(145, 121, 94, 255), 5);
        skinColors.SetValue(new Color32(114, 94, 71, 255), 6);
        skinColors.SetValue(new Color32(87, 74, 58, 255), 7);
        skinColors.SetValue(new Color32(68, 56, 48, 255), 8);
        skinColors.SetValue(new Color32(171, 118, 95, 255), 9);

        hairColors.SetValue(new Color32(0, 0, 0, 255), 0); //black
        hairColors.SetValue(new Color32(26, 13, 2, 255), 1); //dark brown
        hairColors.SetValue(new Color32(51, 26, 3, 255), 2); //brown
        hairColors.SetValue(new Color32(79, 34, 6, 255), 3); //light brown
        hairColors.SetValue(new Color32(143, 83, 10, 255), 4); //dirty blonde
        hairColors.SetValue(new Color32(212, 129, 28, 255), 5); // strawberry blonde
        hairColors.SetValue(new Color32(219, 183, 20, 255), 6); // blonde
        hairColors.SetValue(new Color32(242, 232, 136, 255), 7); // light blonde
        hairColors.SetValue(new Color32(94, 94, 94, 255), 8); // grey
        hairColors.SetValue(new Color32(240, 240, 240, 255), 9); // white
        hairColors.SetValue(new Color32(72, 95, 148, 255), 10); // blue
        hairColors.SetValue(new Color32(103, 72, 148, 255), 11); // purple

        eyeColors.SetValue(new Color32(224, 224, 224, 255), 0); // grey
        eyeColors.SetValue(new Color32(163, 115, 85, 255), 1); // brown
        eyeColors.SetValue(new Color32(217, 198, 89, 255), 2); // hazel
        eyeColors.SetValue(new Color32(179, 227, 125, 255), 3); // green
        eyeColors.SetValue(new Color32(125, 153, 255, 255), 4); // dark blue
        eyeColors.SetValue(new Color32(125, 255, 248, 255), 5); // blue
        eyeColors.SetValue(new Color32(255, 125, 136, 255), 6); // red
        eyeColors.SetValue(new Color32(253, 255, 145, 255), 7); // yellow
        eyeColors.SetValue(new Color32(206, 128, 255, 255), 8); // purple
    }

    private void OnEnable()
    {
        avatar.CharacterUpdated.AddListener(Updated);

        //body sliders
        heightSlider.onValueChanged.AddListener(SliderUpdate);
        upperWeightSlider.onValueChanged.AddListener(SliderUpdate);
        lowerWeightSlider.onValueChanged.AddListener(SliderUpdate);
        upperMuscleSlider.onValueChanged.AddListener(SliderUpdate);
        lowerMuscleSlider.onValueChanged.AddListener(SliderUpdate);
        breastSlider.onValueChanged.AddListener(SliderUpdate);
        feetSlider.onValueChanged.AddListener(SliderUpdate);
        armSlider.onValueChanged.AddListener(SliderUpdate);
        forearmSlider.onValueChanged.AddListener(SliderUpdate);
        gluteusSlider.onValueChanged.AddListener(SliderUpdate);
        legSeparationSlider.onValueChanged.AddListener(SliderUpdate);
        legsSizeSlider.onValueChanged.AddListener(SliderUpdate);
        bellySlider.onValueChanged.AddListener(SliderUpdate);
        waistSlider.onValueChanged.AddListener(SliderUpdate);

        //head sliders
        headSizeSlider.onValueChanged.AddListener(SliderUpdate);
        headWidthSlider.onValueChanged.AddListener(SliderUpdate);
        neckThicknessSlider.onValueChanged.AddListener(SliderUpdate);
        earsPosSlider.onValueChanged.AddListener(SliderUpdate);
        earsRotSlider.onValueChanged.AddListener(SliderUpdate);
        earsSizeSlider.onValueChanged.AddListener(SliderUpdate);

        //face sliders
        cheekPosSlider.onValueChanged.AddListener(SliderUpdate);
        cheekSizeSlider.onValueChanged.AddListener(SliderUpdate);
        foreheadPosSlider.onValueChanged.AddListener(SliderUpdate);
        foreheadSizeSlider.onValueChanged.AddListener(SliderUpdate);
        eyeRotSlider.onValueChanged.AddListener(SliderUpdate);
        eyeSizeSlider.onValueChanged.AddListener(SliderUpdate);
        eyeSpacingSlider.onValueChanged.AddListener(SliderUpdate);
        lipsSizeSlider.onValueChanged.AddListener(SliderUpdate);
        mouthSizeSlider.onValueChanged.AddListener(SliderUpdate);
        lowCheekPosSlider.onValueChanged.AddListener(SliderUpdate);
        lowCheekPronouncedSlider.onValueChanged.AddListener(SliderUpdate);
        mandibleSizeSlider.onValueChanged.AddListener(SliderUpdate);
        jawPosSlider.onValueChanged.AddListener(SliderUpdate);
        jawSizeSlider.onValueChanged.AddListener(SliderUpdate);
        chinPosSlider.onValueChanged.AddListener(SliderUpdate);
        chinPronouncedSlider.onValueChanged.AddListener(SliderUpdate);
        chinSizeSlider.onValueChanged.AddListener(SliderUpdate);
        noseCurveSlider.onValueChanged.AddListener(SliderUpdate);
        noseFlattenSlider.onValueChanged.AddListener(SliderUpdate);
        noseInclinationSlider.onValueChanged.AddListener(SliderUpdate);
        nosePosSlider.onValueChanged.AddListener(SliderUpdate);
        nosePronouncedSlider.onValueChanged.AddListener(SliderUpdate);
        noseSizeSlider.onValueChanged.AddListener(SliderUpdate);
        noseWidthSlider.onValueChanged.AddListener(SliderUpdate);
    }

    void Updated(UMAData data)
    {
        dna = avatar.GetDNA();

        //body
        heightSlider.value = dna["height"].Get();
        upperWeightSlider.value = dna["upperWeight"].Get();
        lowerWeightSlider.value = dna["lowerWeight"].Get();
        upperMuscleSlider.value = dna["upperMuscle"].Get();
        lowerMuscleSlider.value = dna["lowerMuscle"].Get();
        breastSlider.value = dna["breastSize"].Get();
        feetSlider.value = dna["feetSize"].Get();
        armSlider.value = dna["armWidth"].Get();
        forearmSlider.value = dna["forearmWidth"].Get();
        gluteusSlider.value = dna["gluteusSize"].Get();
        legSeparationSlider.value = dna["legSeparation"].Get();
        legsSizeSlider.value = dna["legsSize"].Get();
        bellySlider.value = dna["belly"].Get();
        waistSlider.value = dna["waist"].Get();

        //head
        headSizeSlider.value = dna["headSize"].Get();
        headWidthSlider.value = dna["headWidth"].Get();
        neckThicknessSlider.value = dna["neckThickness"].Get();
        earsPosSlider.value = dna["earsPosition"].Get();
        earsRotSlider.value = dna["earsRotation"].Get();
        earsSizeSlider.value = dna["earsSize"].Get();

        //face
        cheekPosSlider.value = dna["cheekPosition"].Get();
        cheekSizeSlider.value = dna["cheekSize"].Get();
        foreheadPosSlider.value = dna["foreheadPosition"].Get();
        foreheadSizeSlider.value = dna["foreheadSize"].Get();
        eyeRotSlider.value = dna["eyeRotation"].Get();
        eyeSizeSlider.value = dna["eyeSize"].Get();
        eyeSpacingSlider.value = dna["eyeSpacing"].Get();
        lipsSizeSlider.value = dna["lipsSize"].Get();
        mouthSizeSlider.value = dna["mouthSize"].Get();
        lowCheekPosSlider.value = dna["lowCheekPosition"].Get();
        lowCheekPronouncedSlider.value = dna["lowCheekPronounced"].Get();
        mandibleSizeSlider.value = dna["mandibleSize"].Get();
        jawPosSlider.value = dna["jawsPosition"].Get();
        jawSizeSlider.value = dna["jawsSize"].Get();
        chinPosSlider.value = dna["chinPosition"].Get();
        chinPronouncedSlider.value = dna["chinPronounced"].Get();
        chinSizeSlider.value = dna["chinSize"].Get();
        noseCurveSlider.value = dna["noseCurve"].Get();
        noseFlattenSlider.value = dna["noseFlatten"].Get();
        noseInclinationSlider.value = dna["noseInclination"].Get();
        nosePosSlider.value = dna["nosePosition"].Get();
        nosePronouncedSlider.value = dna["nosePronounced"].Get();
        noseSizeSlider.value = dna["noseSize"].Get();
        noseWidthSlider.value = dna["noseWidth"].Get();
    }

    public void ChangeSex()
    {
        if (avatar.activeRace.name == "HumanMaleHD")
        {
            facialHairSelector.SetActive(false);
            maleHairSelector.SetActive(false);
            femaleHairSelector.SetActive(true);
            avatar.ClearSlot("Hair");
            avatar.ClearSlot("Beard");
            avatar.ChangeRace("HumanFemaleHD");
            avatar.SetSlot("Hair", femaleHairStyles[femaleHairIndex]);
        }
        else
        {
            facialHairSelector.SetActive(true);
            maleHairSelector.SetActive(true);
            femaleHairSelector.SetActive(false);
            avatar.ClearSlot("Hair");
            avatar.ClearSlot("Beard");
            avatar.ChangeRace("HumanMaleHD");
            avatar.SetSlot("Hair", maleHairStyles[maleHairIndex]);
            avatar.SetSlot("Beard", facialHairStyles[facialHairIndex]);
        }
        avatar.BuildCharacter();
    }

    public void ChangeSkinTone(bool increase)
    {
        skinIndex = increase ? skinIndex + 1 : skinIndex - 1;
        if (skinIndex == skinColors.Length)
            skinIndex = 0;
        if (skinIndex == -1)
            skinIndex = skinColors.Length - 1;

        avatar.SetColor("Skin", skinColors[skinIndex]);
        avatar.UpdateColors(true);
    }

    public void ChangeHairColor(bool increase)
    {
        hairColorIndex = increase ? hairColorIndex + 1 : hairColorIndex - 1;
        if (hairColorIndex == hairColors.Length)
            hairColorIndex = 0;
        if (hairColorIndex == -1)
            hairColorIndex = hairColors.Length - 1;

        avatar.SetColor("Hair", hairColors[hairColorIndex]);
        avatar.UpdateColors(true);
    }

    public void ChangeEyeColor(bool increase)
    {
        eyeColorIndex = increase ? eyeColorIndex + 1 : eyeColorIndex - 1;
        if (eyeColorIndex == eyeColors.Length)
            eyeColorIndex = 0;
        if (eyeColorIndex == -1)
            eyeColorIndex = eyeColors.Length - 1;

        avatar.SetColor("Eyes", eyeColors[eyeColorIndex]);
        avatar.UpdateColors(true);
    }

    public void ChangeHair(bool increase)
    {
        if (avatar.activeRace.name == "HumanMaleHD")
        {
            maleHairIndex = increase ? maleHairIndex + 1 : maleHairIndex - 1;
            if (maleHairIndex == maleHairStyles.Count)
                maleHairIndex = 0;
            else if (maleHairIndex == -1)
                maleHairIndex = maleHairStyles.Count - 1;

            if (maleHairStyles[maleHairIndex] == "None")
                avatar.ClearSlot("Hair");
            else
                avatar.SetSlot("Hair", maleHairStyles[maleHairIndex]);
        }
        else if (avatar.activeRace.name == "HumanFemaleHD")
        {
            femaleHairIndex = increase ? femaleHairIndex + 1 : femaleHairIndex - 1;
            if (femaleHairIndex == femaleHairStyles.Count)
                femaleHairIndex = 0;
            else if (femaleHairIndex == -1)
                femaleHairIndex = femaleHairStyles.Count - 1;

            if (femaleHairStyles[femaleHairIndex] == "None")
                avatar.ClearSlot("Hair");
            else
                avatar.SetSlot("Hair", femaleHairStyles[femaleHairIndex]);
        }


        avatar.BuildCharacter();
    }

    public void ChangeFacialHair(bool increase)
    {
        facialHairIndex = increase ? facialHairIndex + 1 : facialHairIndex - 1;
        if (facialHairIndex == facialHairStyles.Count)
            facialHairIndex = 0;
        else if (facialHairIndex == -1)
            facialHairIndex = facialHairStyles.Count - 1;

        if (facialHairStyles[facialHairIndex] == "None")
            avatar.ClearSlot("Beard");
        else
            avatar.SetSlot("Beard", facialHairStyles[facialHairIndex]);

        avatar.BuildCharacter();
    }

    private void SliderUpdate(float val)
    {
        dna[dnaString].Set(val);
        avatar.BuildCharacter();
    }

    public void SetDnaString(string dnaStr)
    {
        dnaString = dnaStr;
    }

    public void SaveRecipe()
    {
        myRecipe = avatar.GetCurrentRecipe();
        File.WriteAllText(Application.persistentDataPath + "/umaData.txt", myRecipe);
    }

    public void PlayerUMALoad()
    {
        if (File.Exists(Application.persistentDataPath + "/umaData.txt"))
        {
            Debug.Log("Loading UMA Data...");
            avatar.ClearSlots();
            avatar.LoadFromRecipeString(File.ReadAllText(Application.persistentDataPath + "/umaData.txt"));
        }
    }
}
