using System.Collections;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using static NameManager;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance;

    private List<ISaveable> objectsToSave = new List<ISaveable>();
    private Dictionary<int, object> _states;
    private List<int> _idList = new List<int>();

    private Coroutine _coroutine;
    private int _itemCounter = 0;

    private string rootPath = "G:/Unity_projects/Necromancer/Necromancer/Builds";
    private string _directory = "/SaveData/";
    private string _fileName = "Save.txt";


    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F6) == true)
        {
            DeleteSaveFile();
        }
    }

    private void DeleteSaveFile()
    {
        string savePath = rootPath + _directory;
        //string savePath = Application.persistentDataPath + _directory;
        string pathToFile = savePath + _fileName;

        if(Directory.Exists(savePath) == false)
            return;

        if(File.Exists(pathToFile) == false)
            return;

        File.Delete(pathToFile);

        InfotipManager.ShowMessage("Save file deleted.");
    }

    #region SAVING

    public void SaveGame()
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);

        StartCoroutine(SaveGameCRTN());
    }

    public IEnumerator SaveGameCRTN()
    {
        objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());

        string savePath = rootPath + _directory;
        //string savePath = Application.persistentDataPath + _directory;
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
            _states = JsonConvert.DeserializeObject<Dictionary<int, object>>(File.ReadAllText(pathToFile));
            //_states = (Dictionary<int, object>)JsonConvert.SerializeObject();
            stream.Close();

        }

        Debug.Log("Before saving we have count: " + _states.Count);

        _itemCounter = objectsToSave.Count;

        foreach(var saveItem in objectsToSave)
        {
            saveItem.Save(this);
        }

        while(_itemCounter > 0)
        {
            Debug.Log(_itemCounter + " objects to save left.");
            yield return null;
        }

        string serializedStates = JsonConvert.SerializeObject(_states);
        //File.WriteAllText(pathToFile, serializedStates);

        using(TextWriter writer = new StreamWriter(pathToFile, false))
        {
            writer.WriteLine(serializedStates);
            writer.Close();
        }

        InfotipManager.ShowMessage("Game saved.");
        //Debug.Log("Game saved");
    }

    #endregion

    #region LOADING

    public void LoadGame()
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);

        StartCoroutine(LoadGameCRTN());
    }

    public IEnumerator LoadGameCRTN()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        while(SceneManager.GetActiveScene().isLoaded == false)
        {
            Debug.Log("Loading scene");
            yield return null;
        }

        string pathToFile = rootPath + _directory + _fileName;
        //string pathToFile = Application.persistentDataPath + _directory + _fileName;

        if(File.Exists(pathToFile) == false)
        {
            GlobalStorage.instance.StartNewGame();
        }
        else
        {
            //LoadGame();
            Debug.Log("Game loaded");
            InfotipManager.ShowMessage("Game loaded.");
        }
        yield return null;
    }

#endregion


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
