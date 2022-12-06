using System.Collections;
using UnityEngine;
using TMPro;

public class DeltaCost : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private TMP_Text text;

    private float currentAlfa = 0;
    private float step = 0.05f;

    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;
    [SerializeField] private Color currentColor;

    private string positiveMark = "+";

    public void ShowDelta(float value)
    {
        if(canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            text = GetComponentInChildren<TMP_Text>();
        }

        gameObject.SetActive(true);

        canvasGroup.alpha = 1;
        text.text = "";
        currentColor = negativeColor;
        if(value > 0)
        {
            currentColor = positiveColor;
            text.text = positiveMark;
        } 

        text.text += value.ToString();
        text.color = currentColor;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(step);
        yield return new WaitForSecondsRealtime(1f); 

        currentAlfa = 1;

        while(currentAlfa > 0)
        {
            currentAlfa -= step;
            canvasGroup.alpha = currentAlfa;
            yield return delay;
        }

        gameObject.SetActive(false);
    }
}
