using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum WaterTemperature
    {
        Normal,
        Low,
        High
    }

    public WaterTemperature currentWaterTemperature = WaterTemperature.Normal;
    public bool hasHeater = false; // oyuncu marketten alýrsa true yapýlabilir
    public bool hasCooler = false; // Soðutucu varsa sýcak gün etkilenmez
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
        UIManager.Instance?.UpdateTemperatureUI();
        if (currentWaterTemperature == WaterTemperature.Low && !hasHeater)
            Debug.Log(" Bugün su çok soðuk. Büyüme hýzý düþecek. Marketten Isýtýcý almalýsýn.");

        if (currentWaterTemperature == WaterTemperature.High && !hasCooler)
            Debug.Log(" Bugün su çok sýcak. Büyüme hýzý düþecek. Marketten Soðutucu almalýsýn.");

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
            Debug.Log("Otomatik kayýt yapýldý.");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SaveManager.SaveGame();

        if (Input.GetKeyDown(KeyCode.F1))
            SaveManager.LoadGame();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reset komutu çalýþtý: Oyun sýfýrlanýyor...");
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
            date = "GÜN " + currentDay,
            earned = 0f,
            spent = 0f
        };

        dayStats.Add(today);
        //  Su sýcaklýðýný rastgele belirle
        int randomTemp = UnityEngine.Random.Range(0, 100);
        if (randomTemp < 60)
            currentWaterTemperature = WaterTemperature.Normal;
        else if (randomTemp < 85)
            currentWaterTemperature = WaterTemperature.Low;
        else
            currentWaterTemperature = WaterTemperature.High;

        
        Debug.Log($"Yeni gün baþladý: {today.date}");
        MissionManager.Instance.GenerateNewMission();
        UnityEngine.Object.FindFirstObjectByType<UIMarketManager>()?.RefreshDiscountAndMarket();
        UIManager.Instance.UpdateTemperatureUI();

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
        // Sahnedeki aksesuarlarý temizle
        GameObject[] accessories = GameObject.FindGameObjectsWithTag("Accessory");
        foreach (GameObject accessory in accessories)
        {
            GameObject.Destroy(accessory);
        }

        Debug.Log("Oyun sýfýrlandý: Gün 1’den baþlandý.");
    }
}
