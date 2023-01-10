using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static NameManager;

public class InfotipManager : MonoBehaviour
{
    public static InfotipManager instance;

    [Header("Types of Tips")]
    public WarningTip warningTip;
    public Tooltip tooltip;
    public Squadtip squadTip;
    public SkillTip skillTip;
    public HeroUITip heroTip;
    public RuneTip runeTip;
    public CostTip costTip;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    #region SHOWS

    public static void Show(Unit unit)
    {
        if(unit == null) return;

        instance.squadTip.gameObject.SetActive(true);
        instance.squadTip.Init(unit);
    }

    public static void Show(EnemySO enemy)
    {
        if(enemy == null) return;

        instance.squadTip.gameObject.SetActive(true);
        instance.squadTip.Init(enemy);
    }

    public static void Show(MacroAbilitySO skill)
    {
        if(skill == null) return;

        instance.skillTip.gameObject.SetActive(true);
        instance.skillTip.Init(skill);
    }

    public static void Show()
    {
        instance.heroTip.gameObject.SetActive(true);
        instance.heroTip.ShowTip();
    }

    public static void Show(RuneSO rune)
    {
        if(rune == null) return;

        instance.runeTip.gameObject.SetActive(true);
        instance.runeTip.Show(rune);
    }

    public static void Show(string content, string header = "", string status = "")
    {
        instance.tooltip.gameObject.SetActive(true);
        instance.tooltip.SetText(content, header, status);
    }

    public static void ShowWarning(string content)
    {
        instance.warningTip.gameObject.SetActive(true);
        instance.warningTip.Show(content, false);
    }

    public static void ShowMessage(string content)
    {
        instance.warningTip.gameObject.SetActive(true);
        instance.warningTip.Show(content, true);
    }

    public static void Show(BuildingsRequirements requirements)
    {
        instance.costTip.gameObject.SetActive(true);
        instance.costTip.Init(requirements);
    }

    #endregion

    public static void Hide(TipsType tipsType)
    {
        switch(tipsType)
        {
            case TipsType.Unit:
            case TipsType.Enemy:
                instance.squadTip?.gameObject?.SetActive(false);
                break;

            case TipsType.Skill:
                instance.skillTip?.gameObject?.SetActive(false);
                break;

            case TipsType.Hero:
                instance.heroTip?.gameObject?.SetActive(false);
                break;

            case TipsType.Spell:
                break;

            case TipsType.Rune:
                instance.runeTip?.gameObject?.SetActive(false);
                break; 

            case TipsType.Tool:
                instance.tooltip?.gameObject?.SetActive(false);
                break;

            case TipsType.Requirements:
                instance.costTip?.gameObject?.SetActive(false);
                break;

            default:
                break;
        }
    }
}
