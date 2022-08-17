using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public GameObject tooltipPrefab;
    public GameObject tooltip;
    [SerializeField] private float timeToActivate = 1.5f;
    private float touchCounter = 0;
    private bool isActive = false;

    private void Update()
    {
        if(isActive == true)
        {
            if(tooltip == null) isActive = false;           
        }
    }
    private void OnMouseOver()
    {
        if(isActive == false)
        {
            touchCounter += Time.deltaTime;
            if(touchCounter >= timeToActivate)
            {
                Debug.Log("i see the " + gameObject.name);
                isActive = true;
                CreateTooltip();
            }
        }        
    }

    private void CreateTooltip()
    {
        tooltip = Instantiate(tooltipPrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
    }

    private void OnMouseExit()
    {
        touchCounter = 0;
    }
}
