using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class GMInterfaceCastle : MonoBehaviour
{
    private FortressBuildings fortressBuildings;
    private HeroFortress heroFortress;

    [SerializeField] private GameObject castleBlock;
    private RectTransform castleRect;
    [SerializeField] private TMP_Text levelText;

    [SerializeField] private GameObject buildingsWrapper;
    [SerializeField] private List<GameObject> buildingsList;
    private List<Image> buildingsIconsList = new List<Image>();
    private List<TMP_Text> buildingsTermsList = new List<TMP_Text>();
    private List<TooltipTrigger> buildingsTipsList = new List<TooltipTrigger>();

    [SerializeField] private Animator animator;

    public float minWidth = 110f;
    private float itemWidth;
    private float spaceWidth = 0;

    private void Start()
    {
        heroFortress = GlobalStorage.instance.heroFortress;
        fortressBuildings = GlobalStorage.instance.fortressBuildings;
        castleRect = castleBlock.GetComponent<RectTransform>();

        for(int i = 0; i < buildingsList.Count; i++)
        {
            Image[] icon = buildingsList[i].GetComponentsInChildren<Image>(true);
            buildingsIconsList.Add(icon[icon.Length - 1]);

            TMP_Text text = buildingsList[i].GetComponentInChildren<TMP_Text>(true);
            buildingsTermsList.Add(text);

            TooltipTrigger tip = buildingsList[i].GetComponent<TooltipTrigger>();
            buildingsTipsList.Add(tip);
        }

        itemWidth = buildingsList[0].GetComponent<RectTransform>().rect.width;
        HorizontalLayoutGroup layoutGroup = buildingsWrapper.GetComponent<HorizontalLayoutGroup>();
        if(layoutGroup != null) spaceWidth = layoutGroup.spacing;

        Init();
    }

    public void ShowBlock(bool mode)
    {
        castleBlock.SetActive(mode);

        if(mode == true) Init();
    }

    public void Init()
    {
        if(fortressBuildings == null) return;

        castleDataForUI newData = fortressBuildings.GetDataForUI();

        levelText.text = newData.level.ToString();
        animator.SetBool("bLevelBlink", newData.canIBuild);

        foreach(var building in buildingsList)
            building.SetActive(false);

        for(int i = 0; i < newData.constractions.Count; i++)
        {
            buildingsList[i].SetActive(true);
            buildingsIconsList[i].sprite = newData.constractions[i].icon;
            buildingsTermsList[i].text = newData.constractions[i].daysLeft.ToString();
            buildingsTipsList[i].content = newData.constractions[i].constractionName;
        }

        float width = minWidth + (itemWidth + spaceWidth) * newData.constractions.Count;
        castleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public void UpdateCastleStatus()
    {
        Init();
    }

    public void OpenHeroFortressWindow()
    {
        heroFortress.Open(true);
    }
}
