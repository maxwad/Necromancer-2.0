using System.Collections;
using UnityEngine;

public class LoaderUI : MonoBehaviour
{
    [SerializeField] private float FADE_SPEED = 0.005f;

    [SerializeField] private LoadingWindow loadingWindow;
    [SerializeField] private CanvasGroup canvas;

    private bool canICloseScreen = false;
    public bool CanICloseScreen { get => canICloseScreen; set => canICloseScreen = value; }

    public void Open(bool isSlowShowing)
    { 
        canvas.gameObject.SetActive(true);

        if(isSlowShowing == true)
            Fading.instance.Fade(true, canvas);
    }

    public void ShowLogo(bool isProgressBarNeeded)
    {
        loadingWindow.ShowProgressBar(isProgressBarNeeded);
    }

    public void ResetWindow()
    {
        loadingWindow.ResetWindow();
    }

    public void ShowPhase(string message, float loadingStep)
    {
        loadingWindow.ShowNewPhase(message, loadingStep);
    }

    public void StartLoading()
    {
        loadingWindow.StartLoading();
    }

    public void Close()
    {
        Fading.instance.Fade(false, canvas, step: FADE_SPEED, activeMode: false);
    }

    public void ForceClosing()
    {
        StartCoroutine(Closing());
    }

    private IEnumerator Closing()
    {
        while(loadingWindow.IsProgressFinished == false)
        {
            yield return null;
        }

        loadingWindow.ShowProgressBar(false);
        yield return new WaitForSecondsRealtime(0.5f);

        Close();
    }
}
