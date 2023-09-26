using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    public static Fading instance;
    private static List<CanvasGroup> currentFadings = new List<CanvasGroup>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Fade(bool isShowing, CanvasGroup canvasGroup, float step = 0.1f, float delay = 0f, bool activeMode = true)
    {
        StartCoroutine(StartFading(isShowing, false, canvasGroup, step, delay, activeMode));
    }

    public void FadeWhilePause(bool isShowing, CanvasGroup canvasGroup, float step = 0.1f, float delay = 0f, bool activeMode = true)
    {
        StartCoroutine(StartFading(isShowing, true, canvasGroup, step, delay, activeMode));
    }

    private IEnumerator StartFading(bool isShowing, bool timeMode, CanvasGroup canvasGroup, float step, float delay = 0f, bool activeMode = true)
    {
        if(currentFadings.Contains(canvasGroup) == true)
            yield break;
        else
            currentFadings.Add(canvasGroup);

        float pauseMultiplier = 0.25f;
        float currentAlfa;

        if(isShowing == true)
            currentAlfa = 0;
        else
            currentAlfa = 1;

        canvasGroup.alpha = currentAlfa;
        if(isShowing == true)
            canvasGroup.gameObject.SetActive(true);


        if(delay != 0)
        {
            if(timeMode == false)
            {
                yield return new WaitForSeconds(step * delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(step * delay);
            }
        }

        WaitForSeconds pause = new WaitForSeconds(step * pauseMultiplier);
        WaitForSecondsRealtime pauseRT = new WaitForSecondsRealtime(step * pauseMultiplier);

        if(isShowing == true)
        {
            while(currentAlfa < 1)
            {
                currentAlfa += step * 2;
                canvasGroup.alpha = currentAlfa;

                if(timeMode == false)
                    yield return pause;
                else
                    yield return pauseRT;
            }
        }
        else
        {
            while(currentAlfa > 0)
            {
                currentAlfa -= step * 2;
                canvasGroup.alpha = currentAlfa;

                if(timeMode == false)
                    yield return pause;
                else
                    yield return pauseRT;
            }

            if(activeMode == false)
            {
                canvasGroup.gameObject.SetActive(false);
            }
        }

        currentFadings.Remove(canvasGroup);
    }

    public static bool IsFadingWork(CanvasGroup canvasGroup)
    {
        return currentFadings.Contains(canvasGroup);
    }

    public static bool IsFadingWork()
    {
        return currentFadings.Count > 0;
    }
}
