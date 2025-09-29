using System.Collections;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Enums;
using Zenject;

public class SaveLoadManager : MonoBehaviour, IInputableKeys
{
    private InputSystem inputSystem;
    private static SaveLoadManager instance;
    private LoaderUI loaderUI;

    private List<ISaveable> objectsToSave = new List<ISaveable>();
    private Dictionary<int, object> states = new Dictionary<int, object>();

    private Coroutine coroutine;
    private int saveCounter = 0;
    private int loadCounter = 0;

    private string rootPath = "G:/Unity_projects/Necromancer/Necromancer/Builds";
    private string directory = "/SaveData/";
    private string fileName = "Save.txt";

    private bool canILoadNextPart = true;
    private int parallelIdFlag = 100;
    private float loadingStep;

    [Inject]
    public void Construct(InputSystem inputSystem, LoaderUI loaderUI)
    {
        this.inputSystem = inputSystem;
        this.loaderUI = loaderUI;
    }

    private void ReloadInput(InputSystem inputSystem)
    {
        this.inputSystem = inputSystem;
    }

    private void Start()
    {
        bool shouldIRegister = true;
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GlobalStorage.instance.StartGame(true);
        }
        else
        {
            shouldIRegister = false;
            instance.ReloadInput(inputSystem);
            Destroy(gameObject);
        }

        if(shouldIRegister == true)
        {
            RegisterInputKeys();
        }
    }

    public void RegisterInputKeys()
    {
        inputSystem.RegisterInputKeys(KeyActions.SaveGame, this);
        inputSystem.RegisterInputKeys(KeyActions.LoadGame, this);
    }

    public void InputHandling(KeyActions keyAction)
    {
        if(keyAction == KeyActions.SaveGame)
            SaveGame();

        else if(keyAction == KeyActions.LoadGame)
            LoadGame();
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

#if UNITY_EDITOR
        string savePath = rootPath + directory;
#else
        string savePath = Application.persistentDataPath + directory;
#endif

        string pathToFile = savePath + fileName;

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
        if(coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(Saving());
    }

    private IEnumerator Saving()
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());

        string savePath = rootPath + directory;
        //string savePath = Application.persistentDataPath + directory;
        string pathToFile = savePath + fileName;

        if(Directory.Exists(savePath) == false)
            Directory.CreateDirectory(savePath);

        saveCounter = objectsToSave.Count;

        foreach(var saveItem in objectsToSave)
            saveItem.Save(this);

        while(saveCounter > 0)
        {
            Debug.Log(saveCounter + " objects to save left.");
            yield return null;
        }

        FileStream fileStream = new FileStream(pathToFile, FileMode.Create);
        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string serializedStates = JsonConvert.SerializeObject(states, Formatting.Indented, settings);

            writer.Write(serializedStates);
            writer.Close();
        }

        InfotipManager.ShowMessage("Game saved");
        stopwatch.Stop();
        Debug.Log("Game saved (" + stopwatch.ElapsedMilliseconds / 1000.0f + ")");
    }

    #endregion

    #region LOADING

    public void LoadGame()
    {
        if(coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {

#if UNITY_EDITOR
        string pathToFile = rootPath + directory + fileName;
#else
        string pathToFile = Application.persistentDataPath + directory + fileName;
#endif

        if(File.Exists(pathToFile) == false)
        {
            InfotipManager.ShowMessage("You don't have any saves yet.");
            yield break; 
        }

        var stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        loaderUI.ResetWindow();
        loaderUI.Open(true);
        loaderUI.ShowLogo(true);

        while(Fading.IsFadingWork() == true)
            yield return null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        while(SceneManager.GetActiveScene().isLoaded == false)
            yield return null;

        yield return new WaitForSecondsRealtime(0.1f);

        loaderUI.StartLoading();

        GlobalStorage.instance.StartGame(false);
        while(GlobalStorage.instance.isGameLoaded == false)
        {
            Debug.Log("Initializing map");
            yield return null;
        }

        using(StreamReader reader = new StreamReader(pathToFile))
        {
            string json = reader.ReadToEnd();
            states = (json.Length == 0) ? new Dictionary<int, object>() : JsonConvert.DeserializeObject<Dictionary<int, object>>(json);
        }

        loadCounter = states.Count;
        loadingStep = 1 / (float)loadCounter;

        objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>()).OrderBy(ch => ch.GetId()).ToList();

        //objectsToSave = objectsToSave.OrderByDescending(ch => ch.GetId()).ToList();        
        //Debug.Log("Reverse order: " + objectsToSave.First().GetId());

        foreach(var saveItem in objectsToSave)
        {
            if(saveItem.GetId() < parallelIdFlag)
            {
                while(canILoadNextPart == false)
                    yield return null;
            }

            canILoadNextPart = false;
            saveItem.Load(this, states);
        }

        while(loadCounter > 0)
            yield return null;

        stopWatch.Stop();
        Debug.Log("Load complete (" + stopWatch.ElapsedMilliseconds / 1000.0f + ")");

        loaderUI.ForceClosing();

        RegisterInputKeys();

        canILoadNextPart = true; 
    }

#endregion


    public void FillSaveData(int dataId, object dataObject)
    {
        if(states == null || states.ContainsKey(dataId) == false)
            states.Add(dataId, dataObject);
        else
            states[dataId] = dataObject;

        saveCounter--;
    }

    public void LoadDataComplete(string loadMessage)
    {
        loaderUI.ShowPhase("Loading...", loadingStep);
        loadCounter--;
        canILoadNextPart = true;
    }
}
