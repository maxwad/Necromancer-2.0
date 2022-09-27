using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    public static Fading instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Fade(bool fadeMode, CanvasGroup canvasGroup, float step = 0.1f, float delay = 0f)
    {
        StartCoroutine(StartFading(fadeMode, false, canvasGroup, step, delay));
    }

    public void FadeWhilePause(bool fadeMode, CanvasGroup canvasGroup, float step = 0.1f, float delay = 0f)
    {
        StartCoroutine(StartFading(fadeMode, true, canvasGroup, step, delay));
    }

    public IEnumerator StartFading(bool fadeMode, bool timeMode, CanvasGroup canvasGroup, float step, float delay = 0f)
    {
        float pauseMultiplier = 0.25f;

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

        float currentAlfa; 

        if(fadeMode == true)
        {
            currentAlfa = 0;

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
            currentAlfa = 1;

            while(currentAlfa > 0)
            {
                currentAlfa -= step * 2;
                canvasGroup.alpha = currentAlfa;

                if(timeMode == false)
                    yield return pause;
                else
                    yield return pauseRT;
            }
        }
    }
}
