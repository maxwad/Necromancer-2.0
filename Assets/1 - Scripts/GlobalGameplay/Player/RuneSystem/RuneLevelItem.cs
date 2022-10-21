using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuneLevelItem : MonoBehaviour
{
    public TMP_Text levelText;
    public Image hLink;
    public Image vLink;
    public Image bg;

    public Color activeColor;
    public Color inactiveColor;

    public void Init(bool mode, int level)
    {
        if(mode == true)
        {
            if(level == 1) hLink.gameObject.SetActive(false);

            hLink.color = activeColor;
            vLink.color = activeColor;
            bg.color = activeColor;
        }
        else
        {
            hLink.color = inactiveColor;
            vLink.color = inactiveColor;
            bg.color = inactiveColor;
        }

        levelText.text = level.ToString();
    }    
}
