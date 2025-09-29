using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class PushCircleController : MonoBehaviour
{
    private float currentSize = 0;
    private Coroutine coroutine;

    public void Init(float size)
    {
        currentSize = 0;
        coroutine = StartCoroutine(Sizing(size));
    }

    IEnumerator Sizing(float size)
    {
        WaitForSeconds delay = new WaitForSeconds(0.01f);
        while(currentSize <= size)
        {
            currentSize += 0.4f;
            transform.localScale = new Vector3(currentSize, currentSize, 1);
            yield return delay;
        };

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += Disabling;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= Disabling;
    }

    private void Disabling(bool mode)
    {
        if(coroutine != null) StopCoroutine(coroutine);

        gameObject.SetActive(false);
    }
}
