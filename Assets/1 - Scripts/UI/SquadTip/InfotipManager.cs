using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static NameManager;


public class InfotipManager : MonoBehaviour
{
    public static InfotipManager instance;

    [Header("Types of Tips")]
    public Squadtip squadTip;
    public SkillTip skillTip;

    private RectTransform rectTransformSquad;

    private CanvasGroup currentCanvas;
    private CanvasGroup infoCanvas;
    private CanvasGroup squadCanvas;


    private float heigthForUnit = 490f;
    private float heigthForEnemy = 390f;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        squadCanvas = squadTip.GetComponent<CanvasGroup>();
        infoCanvas = skillTip.GetComponent<CanvasGroup>();

        rectTransformSquad = squadTip.gameObject.GetComponent<RectTransform>();
    }

    #region SHOWS

    public static void Show(Unit unit)
    {
        if(unit == null) return;

        instance.currentCanvas = instance.squadCanvas;
        ResizeWindow(instance.heigthForUnit);
        instance.squadTip.Init(unit);
    }

    public static void Show(EnemyController enemy)
    {
        if(enemy == null) return;

        instance.currentCanvas = instance.squadCanvas;
        ResizeWindow(instance.heigthForEnemy);
        instance.squadTip.Init(enemy);
    }

    public static void Show(MacroAbilitySO skill)
    {
        if(skill == null) return;

        instance.skillTip.gameObject.SetActive(true);
        instance.currentCanvas = instance.infoCanvas;
        instance.skillTip.Init(skill);
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
                break;
            case TipsType.Spell:
                break;
            case TipsType.Boost:
                break;
            default:
                break;
        }
    }

    private static void ResizeWindow(float heigth)
    {
        instance.rectTransformSquad.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heigth);
        instance.squadTip.gameObject.SetActive(true);

        Fading.instance.FadeWhilePause(true, instance.currentCanvas);
    }
}
