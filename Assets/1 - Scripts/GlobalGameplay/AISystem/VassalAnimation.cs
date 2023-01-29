using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class VassalAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text actionLabel;

    public void Init(Color castleColor)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = castleColor;
        Activate(false);
    }

    public void Activate(bool activeMode)
    {
        gameObject.SetActive(activeMode);
    }

    public void FlipSprite(bool flipMode)
    {
        spriteRenderer.flipX = flipMode;
    }

    public void ShowAction(string action)
    {
        actionLabel.text = action;
    }
}
