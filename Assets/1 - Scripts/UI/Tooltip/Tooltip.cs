using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Tooltip : MonoBehaviour
{

    public TextMeshProUGUI headerTt;
    public TextMeshProUGUI contentTt;
    public TextMeshProUGUI statusTt;

    public LayoutElement layoutElement;
    public int characterWrapLimit = 80;

    private int headerLength;
    private int contentLength;
    private int statusLength;

    public RectTransform rectTransform;
    private float offset = 20;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        CorrectTooltipPosition();
    }

    private void CorrectTooltipPosition()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = 0;
        float pivotY = 1;
        float offsetX = offset;
        float offsetY = 0;

        if(Screen.width - position.x < rectTransform.rect.width)
        {
            pivotX = 1f;
            offsetX = -offsetX;
        }

        if(position.y - rectTransform.rect.height < 0)
        {
            pivotY = 0f;
        }

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position + new Vector2(offsetX, offsetY);
    }

    public void SetText(string content, string header = "", string status = "")
    {
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
    }

    private void CheckSize()
    {
        headerLength = headerTt.text.Length;
        contentLength = contentTt.text.Length;
        statusLength = statusTt.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit || statusLength > characterWrapLimit) ? true : false;
    }
}
