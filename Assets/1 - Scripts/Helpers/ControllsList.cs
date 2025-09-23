using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class ControllsList : MonoBehaviour
{
    [SerializeField] private GameObject controlls;
    private bool isOpen = false;


    //Button
    public void ShowControlls()
    {
        isOpen = !isOpen;
        controlls.SetActive(isOpen);
    }
}
