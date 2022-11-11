using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpot : MonoBehaviour
{
    [SerializeField] private List<Sprite> spriteList;

    private SpriteRenderer spriteRenderer;

    public float lifeTime = 5;
    private Coroutine coroutine;

    private Color normalColor = Color.white;
    private Color desapperColor = Color.clear;

    private void OnEnable()
    {
        EventManager.EndOfBattle += BackToPool;

        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteList[Random.Range(0, spriteList.Count)];

        coroutine = StartCoroutine(ColorBack(lifeTime));
    }

    private IEnumerator ColorBack(float divider)
    {
        //the bigger divider the slower animation

        float time = 0;

        while(spriteRenderer.color != desapperColor)
        {
            time += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, desapperColor, time / divider);
            yield return new WaitForSeconds(0.1f);
        }

        gameObject.SetActive(false);
    }
    private void BackToPool()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        spriteRenderer.color = normalColor;
        EventManager.EndOfBattle -= BackToPool;
    }
}
