using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class SquadtipManager : MonoBehaviour
{
    public static SquadtipManager instance;

    public Squadtip squadTip;
    private RectTransform rectTransform;
    private CanvasGroup canvas;
    private Coroutine coroutine;
    private float step = 0.1f;

    private float heigthForUnit = 490f;
    private float heigthForEnemy = 390f;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        canvas = squadTip.GetComponent<CanvasGroup>();
        rectTransform = squadTip.gameObject.GetComponent<RectTransform>();
    }

    public static void Show(Unit unit)
    {
        if(unit == null) return;
        ResizeWindow(instance.heigthForUnit);
        instance.squadTip.Init(unit);
    }

    public static void Show(EnemyController enemy)
    {
        if(enemy == null) return;
        ResizeWindow(instance.heigthForEnemy);
        instance.squadTip.Init(enemy);
    }

    public static void Hide()
    {
        instance.squadTip.gameObject.SetActive(false);
    }

    private static void ResizeWindow(float heigth)
    {
        instance.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heigth);
        instance.squadTip.gameObject.SetActive(true);

        if(instance.coroutine != null) instance.StopCoroutine(instance.coroutine);
        instance.coroutine = instance.StartCoroutine(instance.ShowWindow());
    }

    private IEnumerator ShowWindow()
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(step * 0.1f);

        float start = 0;
        float end = 1;

        instance.canvas.alpha = start;

        while(instance.canvas.alpha < end)
        {
            instance.canvas.alpha += step;
            yield return delay;
        }
    }
}
