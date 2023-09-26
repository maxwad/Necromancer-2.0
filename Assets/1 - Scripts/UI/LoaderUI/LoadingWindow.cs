using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class LoadingWindow : MonoBehaviour
{
    [SerializeField] private float FADE_SPEED = 0.01f;
    [SerializeField] private float LOADING_SPEED = 0.02f;
    [SerializeField] private string LOADING_TEXT = "Loading..";

    [SerializeField] private CanvasGroup progressBarBlock;
    [SerializeField] private Image loadbar;
    [SerializeField] private TMP_Text percent;
    [SerializeField] private TMP_Text phase;

    private bool isProgressFinished = false;

    public bool IsProgressFinished { 
        get => isProgressFinished; 
        set => isProgressFinished = value; 
    }

    private Coroutine coroutine;
    private WaitForSecondsRealtime waitStep = new WaitForSecondsRealtime(0.02f);
    private float currentLoadingPercent = 0;


    public void ShowProgressBar(bool showMode)
    {
        Fading.instance.Fade(showMode, progressBarBlock, step: FADE_SPEED, activeMode: showMode);
    }

    public void ResetWindow()
    {
        progressBarBlock.gameObject.SetActive(true);
        loadbar.fillAmount = 0;
        currentLoadingPercent = 0;
        percent.text = "";
        phase.text = LOADING_TEXT;
    }

    public void ShowNewPhase(string message, float loadingStep)
    { 
        phase.text = message;
        currentLoadingPercent += loadingStep;
    }

    public void StartLoading()
    {
        if(coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(ShowProgress());
    }

    private IEnumerator ShowProgress()
    {
        IsProgressFinished = false;

        while(loadbar.fillAmount < 1f)
        {
            loadbar.fillAmount += LOADING_SPEED;            
            percent.text = Mathf.CeilToInt(loadbar.fillAmount * 100) + "%";
            yield return waitStep;

            while(currentLoadingPercent < loadbar.fillAmount)
            {
                yield return null;
            }
        }

        IsProgressFinished = true;
    }
}
