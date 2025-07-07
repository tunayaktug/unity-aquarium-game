using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static void SaveGame()
    {
        SaveData data = new SaveData();

        data.playerMoney = UIManager.Instance.playerMoney;
        data.currentDay = GameManager.Instance.currentDay;
        data.totalSpent = GameManager.Instance.totalSpent;
        data.totalEarned = GameManager.Instance.totalEarned;
        data.dayStats = GameManager.Instance.dayStats;
        data.completedMissions = MissionManager.Instance.completedMissions;
        data.cleanliness = Object.FindFirstObjectByType<AquariumManager>().cleanliness;
        data.hasFilterSystem = GameManager.Instance.hasFilterSystem;
        data.hasAutoFeeder = GameManager.Instance.hasAutoFeeder;


        FishInfo[] allFish = Object.FindObjectsByType<FishInfo>(FindObjectsSortMode.None);
        foreach (FishInfo fish in allFish)
        {
            FishSaveData f = new FishSaveData
            {
                fishName = fish.fishName,
                hunger = fish.hunger,
                health = fish.health,
                scale = fish.transform.localScale.x,
                prefabName = fish.name.Replace("(Clone)", "").Trim(),
                position = fish.transform.position
            };
            data.fishes.Add(f);
        }
        data.hasHeater = GameManager.Instance.hasHeater;
        data.hasCooler = GameManager.Instance.hasCooler;
        GameObject[] placedAccessories = GameObject.FindGameObjectsWithTag("Accessory");
        foreach (var accessory in placedAccessories)
        {
            data.placedAccessoryNames.Add(accessory.name.Replace("(Clone)", "").Trim());
            data.placedAccessoryPositions.Add(accessory.transform.position);
            data.placedAccessoryRotations.Add(accessory.transform.rotation);
        }


        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        Debug.Log("Oyun kaydedildi.");
    }

    public static void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (!File.Exists(path))
        {
            Debug.LogWarning("Kayýt dosyasý bulunamadý. Baþlangýç paketi veriliyor.");
            GiveStarterFish();
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        GameManager.Instance.hasHeater = data.hasHeater;
        GameManager.Instance.hasCooler = data.hasCooler;
        GameManager.Instance.hasFilterSystem = data.hasFilterSystem;
        GameManager.Instance.hasAutoFeeder = data.hasAutoFeeder;


        UIManager.Instance.playerMoney = data.playerMoney;
        GameManager.Instance.currentDay = data.currentDay;
        GameManager.Instance.totalSpent = data.totalSpent;
        GameManager.Instance.totalEarned = data.totalEarned;
        GameManager.Instance.dayStats = data.dayStats;
        GameManager.Instance.today = data.dayStats[data.dayStats.Count - 1];
        Object.FindFirstObjectByType<AquariumManager>().cleanliness = data.cleanliness;
        UIManager.Instance.UpdateMoneyUI();
        for (int i = 0; i < data.placedAccessoryNames.Count; i++)
        {
            string prefabName = data.placedAccessoryNames[i];
            Vector3 pos = data.placedAccessoryPositions[i];
            Quaternion rot = Quaternion.identity;

            if (i < data.placedAccessoryRotations.Count)
                rot = data.placedAccessoryRotations[i];

            GameObject prefab = Resources.Load<GameObject>("Accessories/" + prefabName);
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab, pos, rot); 
                go.tag = "Accessory";
            }
        }




        FishInfo[] existing = Object.FindObjectsByType<FishInfo>(FindObjectsSortMode.None);
        foreach (FishInfo fish in existing)
            Destroy(fish.gameObject);

        
        foreach (var fishData in data.fishes)
        {
            GameObject prefab = Resources.Load<GameObject>("FishPrefabs/" + fishData.prefabName);
            if (prefab == null) continue;

            GameObject go = Instantiate(prefab, fishData.position, Quaternion.identity);
            FishInfo fi = go.GetComponent<FishInfo>();
            fi.fishName = fishData.fishName;
            fi.hunger = fishData.hunger;
            fi.health = fishData.health;
            go.transform.localScale = Vector3.one * fishData.scale;

            
            FishGrowth growth = go.GetComponent<FishGrowth>();
            if (growth != null)
                growth.isLoaded = true;
        }

     
        MissionManager.Instance.LoadCompletedMissions();
        bool missionLoaded = MissionManager.Instance.LoadMissionIfExists();

        if (missionLoaded)
        {
          
            string key = "MissionCompleted_" + GameManager.Instance.currentDay;
            bool completed = PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) == 1;

            var m = MissionManager.Instance.currentMission;
            int progress = Mathf.Min(m.getProgressFunc(), m.requiredAmount);
            UIManager.Instance.UpdateMissionUI(
                m.description,
                progress,
                m.requiredAmount,
                completed
            );
        }
        else
        {
            
            MissionManager.Instance.GenerateNewMission();
        }

        
        UIManager.Instance.UpdateMissionHistoryUI(BuildFullMissionHistory());

        Debug.Log("Oyun yüklendi.");
        UIManager.Instance.ShowPopup("Oyun baþarýyla yüklendi!");
    }

   
    private static List<string> BuildFullMissionHistory()
    {
        var history = new List<string>();
        int today = GameManager.Instance.currentDay;

        for (int day = 1; day <= today; day++)
        {
            string descKey = "SavedMissionDesc_" + day;
            if (!PlayerPrefs.HasKey(descKey))
                continue;

            string desc = PlayerPrefs.GetString(descKey);
            bool completed = PlayerPrefs.HasKey("MissionCompleted_" + day)
                             && PlayerPrefs.GetInt("MissionCompleted_" + day) == 1;

            
            int reward = 0, required = 0;
            switch (desc)
            {
                case "Balýk sat":
                    reward = 100; required = 3;
                    break;
                case "Balýk besle":
                    reward = 75; required = 2;
                    break;
            }

            if (completed)
            {
                history.Add($"Gün {day}: {desc}  +${reward}");
            }
            else
            {
            
                int prog = 0;
                if (day == today)
                {
                    if (desc == "Balýk sat")
                        prog = GameManager.Instance.today.fishSold;
                    else if (desc == "Balýk besle")
                        prog = GameManager.Instance.today.fishFed;
                }
                history.Add($"Gün {day}: {desc}  ({prog}/{required})");
            }
        }

        return history;
    }

    public static void GiveStarterFish()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject prefab = Resources.Load<GameObject>("FishPrefabs/Anglerfish");
            if (prefab != null)
            {
                Vector3 spawnPos = Object.FindFirstObjectByType<UIMarketManager>()
                                     .fishSpawnPoint.position
                                   + new Vector3(Random.Range(-1f, 1f), 0, 0);
                Instantiate(prefab, spawnPos, Quaternion.identity);
            }
        }

        UIManager.Instance.playerMoney = 100f;
        GameManager.Instance.currentDay = 1;
        GameManager.Instance.totalEarned = 0f;
        GameManager.Instance.totalSpent = 0f;
        GameManager.Instance.today = new DailyStats { date = "Gün 1", earned = 0f, spent = 0f };
        GameManager.Instance.dayStats = new List<DailyStats> { GameManager.Instance.today };
        UIManager.Instance.UpdateMoneyUI();

        Debug.Log("Baþlangýç balýklarý eklendi.");
        UIManager.Instance.ShowPopup("Hoþ geldin! Sana özel 3 balýkla baþlýyorsun");
    }
}
