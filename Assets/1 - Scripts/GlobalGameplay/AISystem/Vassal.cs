using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Vassal : MonoBehaviour
{
    private EnemyCastle myCastle;
    private VassalAnimation animScript;

    private Vector3 startPosition;

    private void Start()
    {
        StartCoroutine(WaitForEnterPoint());
    }

    private IEnumerator WaitForEnterPoint()
    {
        while(startPosition == Vector3.zero)
        {
            yield return null;
            startPosition = myCastle.GetStartPosition();
        }

        transform.position = startPosition;
    }

    public void Init(EnemyCastle castle, Color castleColor, string name)
    {
        myCastle = castle;

        GetComponent<TooltipTrigger>().content = name;

        animScript = GetComponent<VassalAnimation>();
        animScript.Init(castleColor);

    }
    
    public void StartAction()
    {
        animScript.Activate(true);
    }

    private void GetArmy()
    {

    }
}
