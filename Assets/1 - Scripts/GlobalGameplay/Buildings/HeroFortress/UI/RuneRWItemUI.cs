using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class RuneRWItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image bg;
    [SerializeField] private GameObject countBG;
    [SerializeField] private TMP_Text countText;

    [SerializeField] private List<Color> levelColors;

    [SerializeField] private InfotipTrigger infotip;

    private RuneSO rune;
    private RuneWorkroom workroom;

    public void Init(RuneWorkroom room, RuneSO currentRune, bool isStoreRune, string count = "")
    {
        if(workroom == null)
        {
            workroom = room;
        }

        rune = currentRune;
        icon.sprite = rune.activeIcon;
        bg.color = (isStoreRune == true) ? levelColors[0] : levelColors[rune.level];

        countBG.SetActive(!isStoreRune);
        countText.text = count;

        infotip.SetRune(currentRune);
    }
}
