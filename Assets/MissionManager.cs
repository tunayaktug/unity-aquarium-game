using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> items;
        public SerializationWrapper(List<T> items) => this.items = items;
    }

    
    public List<string> completedMissions = new();

    
    public static MissionManager Instance { get; private set; }

  
    public DailyMission currentMission;
    public bool missionCompleted;

    private void Awake()
    {
        Instance = this;
        LoadCompletedMissions();
    }

    private void Start()
    {
        if (!LoadMissionIfExists())
            GenerateNewMission();
        else
            UpdateMissionPanel();

        UIManager.Instance.UpdateMissionHistoryUI(BuildFullMissionHistory());
    }

    private void Update()
    {
        if (currentMission == null)
            return;

        int progress = currentMission.getProgressFunc();
        bool completed = currentMission.checkCompletion();

        UIManager.Instance.UpdateMissionUI(
            currentMission.description,
            Mathf.Min(progress, currentMission.requiredAmount),
            currentMission.requiredAmount,
            completed
        );

        if (!missionCompleted && completed)
        {
            missionCompleted = true;
            UIManager.Instance.playerMoney += currentMission.reward;
            GameManager.Instance.today.earned += currentMission.reward;
            UIManager.Instance.UpdateMoneyUI();
            UIManager.Instance.ShowPopup($"Görev tamamlandý! +{currentMission.reward}$");

            SaveMissionCompleted();

            string entry = $"Gün {GameManager.Instance.currentDay}: {currentMission.description}  +${currentMission.reward}";
            if (!completedMissions.Contains(entry))
            {
                completedMissions.Add(entry);
                SaveCompletedMissions();
            }

            UIManager.Instance.UpdateMissionHistoryUI(BuildFullMissionHistory());
        }
    }

    public void GenerateNewMission()
    {
        var missions = new List<DailyMission>
        {
            new("Balýk sat",
                () => GameManager.Instance.today.fishSold >= 3,
                () => GameManager.Instance.today.fishSold,
                3, 100),
            new("Balýk besle",
                () => GameManager.Instance.today.fishFed >= 2,
                () => GameManager.Instance.today.fishFed,
                2, 75)
        };

        currentMission = missions[UnityEngine.Random.Range(0, missions.Count)];
        missionCompleted = false;

        SaveCurrentMission();
        SaveMissionCompleted();
        UpdateMissionPanel();
    }

    private void UpdateMissionPanel()
    {
        int progress = currentMission.getProgressFunc();
        UIManager.Instance.UpdateMissionUI(
            currentMission.description,
            Mathf.Min(progress, currentMission.requiredAmount),
            currentMission.requiredAmount,
            missionCompleted
        );
    }

    private void SaveCurrentMission()
    {
        int day = GameManager.Instance.currentDay;
        PlayerPrefs.SetString($"SavedMissionDesc_{day}", currentMission.description);
        PlayerPrefs.Save();
    }

    public bool LoadMissionIfExists()
    {
        int day = GameManager.Instance.currentDay;
        string key = $"SavedMissionDesc_{day}";
        if (!PlayerPrefs.HasKey(key))
            return false;

        currentMission = CreateMissionByDescription(PlayerPrefs.GetString(key));
        if (currentMission == null)
            return false;

        LoadMissionCompleted();
        return true;
    }

    private DailyMission CreateMissionByDescription(string desc) => desc switch
    {
        "Balýk sat" => new("Balýk sat",
                             () => GameManager.Instance.today.fishSold >= 3,
                             () => GameManager.Instance.today.fishSold,
                             3, 100),
        "Balýk besle" => new("Balýk besle",
                              () => GameManager.Instance.today.fishFed >= 2,
                              () => GameManager.Instance.today.fishFed,
                              2, 75),
        _ => null
    };

    private void SaveMissionCompleted()
    {
        int day = GameManager.Instance.currentDay;
        PlayerPrefs.SetInt($"MissionCompleted_{day}", missionCompleted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadMissionCompleted()
    {
        int day = GameManager.Instance.currentDay;
        missionCompleted = PlayerPrefs.GetInt($"MissionCompleted_{day}", 0) == 1;
    }

    public void SaveCompletedMissions()
    {
        string json = JsonUtility.ToJson(new SerializationWrapper<string>(completedMissions));
        PlayerPrefs.SetString("CompletedMissions", json);
        PlayerPrefs.Save();
    }

    public void LoadCompletedMissions()
    {
        if (PlayerPrefs.HasKey("CompletedMissions"))
        {
            var wrapper = JsonUtility.FromJson<SerializationWrapper<string>>(
                PlayerPrefs.GetString("CompletedMissions"));
            completedMissions = wrapper.items ?? new();
        }
        else
        {
            completedMissions = new();
        }
    }

    public void ResetMissions()
    {
      
        completedMissions.Clear();
        for (int day = 1; day <= 365; day++)
        {
            PlayerPrefs.DeleteKey($"SavedMissionDesc_{day}");
            PlayerPrefs.DeleteKey($"MissionCompleted_{day}");
        }
        PlayerPrefs.DeleteKey("CompletedMissions");
        PlayerPrefs.Save();

      
        UIManager.Instance.UpdateMissionHistoryUI(BuildFullMissionHistory());
        Debug.Log("Görev kayýtlarý sýfýrlandý.");

        
        foreach (var fish in FindObjectsByType<FishInfo>(FindObjectsSortMode.None))
            Destroy(fish.gameObject);

       
        GameManager.Instance.currentDay = 1;
        GameManager.Instance.totalEarned = 0f;
        GameManager.Instance.totalSpent = 0f;
        GameManager.Instance.today = new DailyStats { date = "Day 1", earned = 0f, spent = 0f };
        GameManager.Instance.dayStats = new List<DailyStats> { GameManager.Instance.today };

      
        var aquarium = FindAnyObjectByType<AquariumManager>();
        if (aquarium != null)
            aquarium.cleanliness = 100f;

     
        UIManager.Instance.playerMoney = 0f;
        UIManager.Instance.UpdateMoneyUI();

      
        SaveManager.GiveStarterFish();

      
        GenerateNewMission();
        UIManager.Instance.UpdateMissionHistoryUI(BuildFullMissionHistory());
        UIManager.Instance.ShowPopup("Oyun sýfýrlandý. Yeni baþlangýç yapýyorsun!");
    }

    private List<string> BuildFullMissionHistory()
    {
        var history = new List<string>();
        int today = GameManager.Instance.currentDay;

        for (int day = 1; day <= today; day++)
        {
            string key = $"SavedMissionDesc_{day}";
            if (!PlayerPrefs.HasKey(key)) continue;

            string desc = PlayerPrefs.GetString(key);
            bool completed = PlayerPrefs.GetInt($"MissionCompleted_{day}", 0) == 1;

            int reward = desc == "Balýk sat" ? 100 : desc == "Balýk besle" ? 75 : 0;
            int required = desc == "Balýk sat" ? 3 : desc == "Balýk besle" ? 2 : 0;

            if (completed)
            {
                history.Add($"Gün {day}: {desc}  +${reward}");
            }
            else
            {
                int prog = 0;
                if (day == today)
                    prog = desc == "Balýk sat"
                        ? GameManager.Instance.today.fishSold
                        : GameManager.Instance.today.fishFed;

                history.Add($"Gün {day}: {desc}  ({prog}/{required})");
            }
        }

        return history;
    }
}
