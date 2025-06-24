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
        currentDay = 1;

        today = new DailyStats
        {
            date = "Day " + currentDay,
            earned = 0f,
            spent = 0f
        };

        dayStats.Add(today);
    }

    private void Update()
    {
        dayTimer += Time.deltaTime;

        if (dayTimer >= dayLength)
        {
            StartNewGameDay();
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

        Debug.Log($"Yeni gün baþladý: {today.date}");
    }

    public float NetProfit => totalEarned - totalSpent;
}
