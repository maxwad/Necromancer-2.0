using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class FireballsController : MonoBehaviour
{
    private bool isWatching = false;
    private SpriteRenderer spriteRenderer;
    private Coroutine appearing;
    private Coroutine attack;


    private void OnEnable()
    {
        EventManager.EndOfBattle += AbortCoroutine;

        Init();
    }

    public void Init()
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        appearing = StartCoroutine(Fade(false));
        isWatching = true;
        attack = StartCoroutine(Attack());
    }

    private void Update()
    {
        if(isWatching == true && GlobalStorage.instance.isGlobalMode == false) 
        {
            Vector3 targ = GlobalStorage.instance.hero.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(48, 0, angle));
        }
    }

    private IEnumerator Fade(bool fadeMode)
    {
        yield return null;

        float step = 0.01f;
        float startValue = 0f;
        float endValue = 1f;
        float multiplier = 2;

        if(fadeMode == true)
        {
            startValue = 1f;
            endValue = 0f;
            multiplier = -2;
        }

        float currentValue = startValue;

        Color currentColor = spriteRenderer.color;        

        while(currentValue != endValue)
        {
            currentValue += multiplier * step;
            spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentValue);
            yield return new WaitForSeconds(step);
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(2f);

        isWatching = true;

    }

    private void AbortCoroutine()
    {
        isWatching = false;
        if(appearing != null) StopCoroutine(appearing);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= AbortCoroutine;
    }
}
