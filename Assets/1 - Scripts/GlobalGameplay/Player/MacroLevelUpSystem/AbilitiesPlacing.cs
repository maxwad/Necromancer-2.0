using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class AbilitiesPlacing : MonoBehaviour
{
    private Dictionary<PlayersStats, List<MacroAbilitySO>> availableAbilitiesDict = new Dictionary<PlayersStats, List<MacroAbilitySO>>();
    private Dictionary<PlayersStats, List<SkillItem>> skillsUIDict = new Dictionary<PlayersStats, List<SkillItem>>();

    [Header("Build elements")]
    [SerializeField] private GameObject skillsWrapper;
    [SerializeField] private List<GameObject> rowContainers;
    [SerializeField] private GameObject skillBlock;
    [SerializeField] private GameObject skillItem;

    [Header("Parameters")]
    private float heightContainer;

    private float baseWidth;
    private float linkOffset = 20f;
    private float sidesPadding = 5f;

    public void Init(Dictionary<PlayersStats, List<MacroAbilitySO>> abilities)
    {
        availableAbilitiesDict = abilities;

        ResizeContainers();
        FillRows();
    }

    private void ResizeContainers()
    {
        heightContainer = (skillsWrapper.GetComponent<RectTransform>().rect.height - sidesPadding * 2) / rowContainers.Count;
        foreach(var row in rowContainers)
        {
            row.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightContainer);
        }
    }

    private void FillRows()
    {
        baseWidth = skillItem.GetComponent<RectTransform>().rect.width;

        float constWidth = skillsWrapper.GetComponent<RectTransform>().rect.width;
        float currentWidth = 0;
        int currentRow = 0;

        foreach(var skillSeries in availableAbilitiesDict)
        {            
            float width = sidesPadding * 2 + baseWidth * skillSeries.Value.Count + linkOffset * (skillSeries.Value.Count - 1);
            if(width + currentWidth > constWidth)
            {
                if(currentRow + 1 < rowContainers.Count)
                {
                    currentRow++;
                    currentWidth = 0;
                }
                else
                {
                    Debug.Log("ERROR!");
                    return;
                }
            }

            GameObject newSkillBlock = Instantiate(skillBlock);
            newSkillBlock.transform.SetParent(rowContainers[currentRow].transform, false);
            newSkillBlock.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            newSkillBlock.GetComponentInChildren<Wrapper>().gameObject
                .GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            currentWidth += width;

            newSkillBlock.GetComponentInChildren<TMP_Text>().text = skillSeries.Value[0].serieName;
            float linkWidth = (width - baseWidth * skillSeries.Value.Count) / skillSeries.Value.Count;

            skillsUIDict[skillSeries.Key] = new List<SkillItem>();

            for(int i = 0; i < skillSeries.Value.Count; i++)
            {
                GameObject newSkillItem = Instantiate(skillItem);
                newSkillItem.transform.SetParent(newSkillBlock.GetComponentInChildren<Wrapper>().gameObject.transform, false);
                SkillItem skillScript = newSkillItem.GetComponent<SkillItem>();

                //Debug.Log("Level " + skillSeries.Value[i].level);
                skillScript.Init(skillSeries.Value[i], i, linkWidth);
                skillsUIDict[skillSeries.Key].Add(skillScript);
            }

            //Debug.Log(skillsUIDict[skillSeries.Key].Count);
        }
    }

    public void MarkSkill(MacroAbilitySO skill)
    {

        foreach(var skillSeries in availableAbilitiesDict)
        {
            if(skillSeries.Key == skill.abilitySeries)
            {
                for(int i = 0; i < skillSeries.Value.Count; i++)
                {
                    if(skillSeries.Value[i].level == skill.level)
                    {
                        foreach(var skillInList in skillsUIDict[skillSeries.Key])
                        {
                            if(skillInList.skill.level == skill.level)
                            {
                                skillInList.MarkAsOpened();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
