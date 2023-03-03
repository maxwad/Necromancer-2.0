using System.Collections;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    private float _saveTime = 0f;
    private float _loadTime = 0f;


    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GlobalStorage.instance.StartGame(true);
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
        _saveTime = Time.time;

        objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());

        string savePath = rootPath + _directory;
        //string savePath = Application.persistentDataPath + _directory;
        string pathToFile = savePath + _fileName;

        if(Directory.Exists(savePath) == false)
            Directory.CreateDirectory(savePath);

        _saveCounter = objectsToSave.Count;

        foreach(var saveItem in objectsToSave)
            saveItem.Save(this);

        while(_saveCounter > 0)
        {
            Debug.Log(_saveCounter + " objects to save left.");
            yield return null;
        }

        FileStream fileStream = new FileStream(pathToFile, FileMode.Create);
        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string serializedStates = JsonConvert.SerializeObject(_states, Formatting.Indented, settings);

            writer.Write(serializedStates);
            writer.Close();
        }

        InfotipManager.ShowMessage("Game saved");
        Debug.Log("Game saved (" + (Time.time - _saveTime) + ")");
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
        string pathToFile = rootPath + _directory + _fileName;
        //string pathToFile = Application.persistentDataPath + _directory + _fileName;

        if(File.Exists(pathToFile) == false)
        {
            InfotipManager.ShowMessage("You don't have any saves yet.");
            yield break; 
        }

        _loadTime = Time.time;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        while(SceneManager.GetActiveScene().isLoaded == false)
        {
            Debug.Log("Loading scene");
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);

        GlobalStorage.instance.StartGame(false);
        while(GlobalStorage.instance.isGameLoaded == false)
        {
            Debug.Log("Initializing map");
            yield return null;
        }

        using(StreamReader reader = new StreamReader(pathToFile))
        {
            string json = reader.ReadToEnd();
            _states = (json.Length == 0) ? new Dictionary<int, object>() : JsonConvert.DeserializeObject<Dictionary<int, object>>(json);
        }

        _loadCounter = _states.Count;

        objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());

        foreach(var saveItem in objectsToSave)
            saveItem.Load(this, _states);

        while(_loadCounter > 0)
        {
            Debug.Log(_loadCounter + " objects to load left.");
            yield return null;
        }

        InfotipManager.ShowMessage("Game loaded.");
        Debug.Log("Game loaded (" + (Time.time - _loadTime) + ")");
    }

#endregion


    public void FillSaveData(int dataId, object dataObject)
    {
        if(_states == null || _states.ContainsKey(dataId) == false)
            _states.Add(dataId, dataObject);
        else
            _states[dataId] = dataObject;

        _saveCounter--;
    }

    public void LoadDataComplete(string loadMessage)
    {
        Debug.Log(loadMessage);
        _loadCounter--;
    }

    public T ConvertToRequiredType<T>(object data)
    {
        JObject tempObject = (JObject)data;
        T convertedData = tempObject.ToObject<T>();
        return convertedData;
    }
}
