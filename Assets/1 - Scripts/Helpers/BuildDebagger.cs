using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class BuildDebagger : MonoBehaviour
{
    public static BuildDebagger instance;
    public GameObject block;
    public RectTransform messageWrapper;
    [SerializeField] private GameObject message;

    private List<GameObject> messages = new List<GameObject>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    public void Show(string text)
    {
        if(block.gameObject.activeInHierarchy == false)
        {
            block.gameObject.SetActive(true);
        }

        GameObject textBlock = Instantiate(message);
        textBlock.GetComponent<TMP_Text>().text = text;
        textBlock.transform.SetParent(messageWrapper, false);
        messages.Add(textBlock);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F12))
        {
            foreach(var item in messages)
            {
                Destroy(item);
            }
        }
    }
}
