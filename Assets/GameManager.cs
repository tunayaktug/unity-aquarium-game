using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float totalSpent = 0f;
    public float totalEarned = 0f;

    public List<DailyStats> dayStats = new List<DailyStats>();
    public DailyStats today;

    public int currentDay = 1;
    public float dayLength = 30f; // 15 dakika = 900f
    private float dayTimer = 0f;


    private float autoSaveTimer = 0f;
    private float autoSaveInterval = 30f; // her 30 saniyede bir

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SaveManager.LoadGame(); 

       
        if (GameManager.Instance.dayStats.Count == 0)
        {
            currentDay = 1;
            today = new DailyStats
            {
                date = "Day " + currentDay,
                earned = 0f,
                spent = 0f
            };

            dayStats.Add(today);
        }
    }

    private void Update()
    {
        dayTimer += Time.deltaTime;

        if (dayTimer >= dayLength)
        {
            StartNewGameDay();
        }

        if (autoSaveTimer >= autoSaveInterval)
        {
            SaveManager.SaveGame();
            autoSaveTimer = 0f;
            Debug.Log("Otomatik kay�t yap�ld�.");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SaveManager.SaveGame();

        if (Input.GetKeyDown(KeyCode.F1))
            SaveManager.LoadGame();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reset komutu �al��t�: Oyun s�f�rlan�yor...");
            GameManager.Instance.ResetGame();
            MissionManager.Instance.ResetMissions();

            UIManager.Instance.playerMoney = 0;
            UIManager.Instance.UpdateMoneyUI();

            MissionManager.Instance.GenerateNewMission();
        }
    }

    void StartNewGameDay()
    {
        dayTimer = 0f;
        currentDay++;

        today = new DailyStats
        {
            date = "DAY " + currentDay,
            earned = 0f,
            spent = 0f
        };

        dayStats.Add(today);

        Debug.Log($"Yeni g�n ba�lad�: {today.date}");
        MissionManager.Instance.GenerateNewMission();
        UnityEngine.Object.FindFirstObjectByType<UIMarketManager>()?.RefreshDiscountAndMarket();


    }

    public float NetProfit => totalEarned - totalSpent;


  
    public void ResetGame()
    {
        currentDay = 1;

        today = new DailyStats
        {
            date = "Day " + currentDay,
            earned = 0f,
            spent = 0f,
            fishSold = 0,
            fishFed = 0
        };

        dayStats.Clear();
        dayStats.Add(today);

        totalEarned = 0f;
        totalSpent = 0f;
        dayTimer = 0f;

        Debug.Log("Oyun s�f�rland�: G�n 1�den ba�land�.");
    }
}
