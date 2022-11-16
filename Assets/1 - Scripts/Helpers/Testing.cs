using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Testing : MonoBehaviour
{
    public class Test
    {
        public int level;
        public GameObject obj;
    }

    private ObjectsPoolManager poolManager;

    private List<Test> list = new List<Test>();
    private Test testVar;
    private Test testVar2;

    private void Start()
    {
        Invoke("Init", 2f);
    }

    void Init()
    {
        poolManager = GlobalStorage.instance.objectsPoolManager;

        for(int i = 0; i < 4; i++)
        {
            Test item = new Test();
            item.level = 1;
            item.obj = poolManager.GetObject((ObjectPool)(i + 6));

            list.Add(item);
        }


        testVar = list[2];
        testVar2 = list[3];
        Show();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            Replace();
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Replace2();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Replace3();
        }
    }

    private void Show()
    {
        Debug.Log("-------------------");
        for(int i = 0; i < list.Count; i++)
        {
            if(list[i] == null)
            {
                Debug.Log("#" + i + " = null");
            }
            else
            {
                Debug.Log("#" + i + " = " + list[i].obj.name + " (level " + list[i].level + ")");

            }

        }

        Debug.Log("test var = " + testVar);

        Debug.Log("test var2 = " + testVar2);

        Debug.Log("-------------------");
    }

    private void Replace()
    {
        testVar = null;
        Show();
    }

    private void Replace2()
    {
        testVar = testVar2;
        Show();
    }

    private void Replace3()
    {
        list[3] = null;
        Show();
    }
}
