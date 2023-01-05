using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Tooltip : MonoBehaviour
{
    private CanvasGroup canvas;
    public TextMeshProUGUI headerTt;
    public TextMeshProUGUI contentTt;
    public TextMeshProUGUI statusTt;

    public LayoutElement layoutElement;
    public int characterWrapLimit = 80;

    private int headerLength;
    private int contentLength;
    private int statusLength;

    public void SetText(string content, string header = "", string status = "")
    {
        if(canvas == null) canvas = GetComponent<CanvasGroup>();

        if(string.IsNullOrEmpty(header))
        {
            headerTt.gameObject.SetActive(false);
        }
        else
        {
            headerTt.gameObject.SetActive(true);
            headerTt.text = header;
        }

        if(string.IsNullOrEmpty(status))
        {
            statusTt.gameObject.SetActive(false);
        }
        else
        {
            statusTt.gameObject.SetActive(true);
            statusTt.text = status;
        }

        contentTt.text = content;

        CheckSize();
        Fading.instance.FadeWhilePause(true, canvas);
    }

    private void CheckSize()
    {
        headerLength = headerTt.text.Length;
        contentLength = contentTt.text.Length;
        statusLength = statusTt.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit || statusLength > characterWrapLimit) ? true : false;
    }
}
