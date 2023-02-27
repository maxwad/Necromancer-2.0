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
    private Dictionary<int, object> _states = new Dictionary<int, object>();
    private List<int> _idList = new List<int>();

    private Coroutine _coroutine;
    private int _saveCounter = 0;
    private int _loadCounter = 0;

    private string rootPath = "G:/Unity_projects/Necromancer/Necromancer/Builds";
    private string _directory = "/SaveData/";
    private string _fileName = "Save.txt";

    public int test = 1;


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

        //Debug.Log("Before saving we have data:");
        //foreach(var item in _states)
        //{
        //    Debug.Log($"{item.Key} have data {item.Value}");
        //}

        _saveCounter = objectsToSave.Count;

        foreach(var saveItem in objectsToSave)
        {
            saveItem.Save(this);
        }

        while(_saveCounter > 0)
        {
            Debug.Log(_saveCounter + " objects to save left.");
            yield return null;
        }

        FileStream fileStream = new FileStream(pathToFile, FileMode.Create);
        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            Debug.Log("SERIALIZE: " + _states.Count);
            string serializedStates = JsonConvert.SerializeObject(_states);
            Debug.Log("STRING: " + serializedStates);
            writer.Write(serializedStates);
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

        yield return new WaitForSecondsRealtime(0.1f);
        test = Random.Range(0, 111);

        string pathToFile = rootPath + _directory + _fileName;
        //string pathToFile = Application.persistentDataPath + _directory + _fileName;

        if(File.Exists(pathToFile) == true)
        {
            using(StreamReader reader = new StreamReader(pathToFile))
            {
                string json = reader.ReadToEnd();
                Debug.Log("STRING: " + json.Length);
                _states = (json.Length == 0) ? new Dictionary<int, object>() : JsonConvert.DeserializeObject<Dictionary<int, object>>(json);
            }

            _loadCounter = _states.Count;

            objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());
            Debug.Log("Type of object: " + _states.GetType());
            foreach(var item in _states)
            {
                //Reward test = (Reward)item.Value;
                Debug.Log("Check types: ");
                Debug.Log("id: " + item.Key.GetType());
                Debug.Log("value: " + item.Value.GetType());
            }

            foreach(var saveItem in objectsToSave)
            {
                //Newtonsoft.Json.Linq.JObject test = ()json;
                    //var values = test.ToObject<Dictionary<string, object>>();
                saveItem.Load(this, _states);
            }

            while(_loadCounter > 0)
            {
                Debug.Log(_loadCounter + " objects to load left.");
                yield return null;
            }

            Debug.Log("Game loaded");
            InfotipManager.ShowMessage("Game loaded.");
        }
        else
        {
            GlobalStorage.instance.StartNewGame();
        }
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
        if(_states == null || _states.ContainsKey(dataId) == false)
            _states.Add(dataId, dataObject);
        else
            _states[dataId] = dataObject;

        _saveCounter--;
        Debug.Log("Saved");
    }

    public void LoadDataComplete(string loadMessage)
    {
        _loadCounter--;
    }
}
