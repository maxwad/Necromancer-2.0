using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class VassalAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

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
}
