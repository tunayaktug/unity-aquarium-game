using System.Collections.Generic;
using UnityEngine;
using static MissionManager;

[System.Serializable]
public class SaveData
{
    public float playerMoney;
    public int currentDay;
    public float totalSpent;
    public float totalEarned;
    public float cleanliness;

    public List<DailyStats> dayStats = new List<DailyStats>();
    public List<FishSaveData> fishes = new List<FishSaveData>();
    public List<string> completedMissions = new List<string>();
    

}
