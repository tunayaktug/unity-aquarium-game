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

    public bool hasHeater;
    public bool hasCooler;

    public List<string> placedAccessoryNames = new List<string>();
    public List<Vector3> placedAccessoryPositions = new List<Vector3>();



}
