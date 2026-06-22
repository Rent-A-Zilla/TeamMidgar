using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage")]
    [SerializeField] private string fileName;

    private GameData gameData;

    private List<IDataPersistence> dataPersistencesObjects;

    private FileDataHandler dataHandler;

    public static bool shouldLoadGame = false;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistencesObjects = FindAllDataPersistenceObjects();
        LoadGame(); 
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        
        if(this.gameData == null)
        {
            NewGame();
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>(); 

        return new List<IDataPersistence>(dataPersistenceObjects);  
    }
}
