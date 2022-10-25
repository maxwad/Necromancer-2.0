using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneLevelWrapper : MonoBehaviour
{
    [SerializeField] private List<RuneLevelItem> levelList;
    private bool isListReversed = false;

    public void Init(float level)
    {
        if(isListReversed == false)
        {
            levelList.Reverse();
            isListReversed = true;
        }

        bool mode;

        for(int i = 0; i < levelList.Count; i++)
        {
            mode = (i + 1 > level) ? false : true;

            levelList[i].Init(mode, i + 1);
        }
    }
}
