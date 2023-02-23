using System.Collections;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SaveLoadManager : MonoBehaviour, IInputableKeys
{
    private InputSystem inputSystem;

    private List<ISaveable> objectsToSave = new List<ISaveable>();
    private Dictionary<int, object> _states;
    private List<int> _idList = new List<int>();

    private Coroutine _coroutine;
    private int _itemCounter = 0;

    private string _directory = "/SaveData/";
    private string _fileName = "Save.txt";

    public void RegisterInputKeys()
    {
        inputSystem = GlobalStorage.instance.inputSystem;
        inputSystem.RegisterInputKeys(KeyActions.SaveGame, this);
        inputSystem.RegisterInputKeys(KeyActions.LoadGame, this);
    }

    public void InputHandling(KeyActions keyAction)
    {
        if(keyAction == KeyActions.SaveGame)
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);

            StartCoroutine(SaveGame());
        }

        else if(keyAction == KeyActions.LoadGame)
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);

            StartCoroutine(LoadGame());
        }
    }

    private void Start()
    {
        RegisterInputKeys();

        LoadGame();
    }

    public IEnumerator SaveGame()
    {
        objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());
        Debug.Log(objectsToSave.Count);

        string savePath = Application.persistentDataPath + _directory;
        string pathToFile = savePath + _fileName;

        if(Directory.Exists(savePath) == false)
            Directory.CreateDirectory(savePath);

        if(File.Exists(pathToFile) == false)
        {
            File.Create(pathToFile);
            _states = new Dictionary<int, object>();
        }
        else
        {
            FileStream stream = File.Open(pathToFile, FileMode.Open);
        }

        _itemCounter = objectsToSave.Count;
        while(_itemCounter > 0)
        {
            yield return null;
        }

        Debug.Log("Game saved");
    }

    public IEnumerator LoadGame()
    {
        string pathToFile = Application.persistentDataPath + _directory + _fileName;

        if(File.Exists(pathToFile) == false)
        {
            GlobalStorage.instance.StartNewGame();
        }
        else
        {
            //LoadGame();
            Debug.Log("Game loaded");
        }
        yield return null;
    }

    public bool CanIUseId(int id)
    {
        if(_idList.Contains(id) == false)
        {
            _idList.Add(id);
            return true;
        }

        return false;
    }

    public void FillSaveData(int dataId, object dataObject)
    {
        if(_states.ContainsKey(dataId) == false)
        {
            _states.Add(dataId, dataObject);
            _itemCounter--;
        }
    }
}
