using UnityEngine;
using System;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{

    public List<string> completedMissions = new List<string>();
    public static MissionManager Instance;

    public DailyMission currentMission;
    private bool missionCompleted = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateNewMission();
    }

    void Update()
    {
        if (currentMission == null) return;

        int current = currentMission.getProgressFunc();
        bool completed = currentMission.checkCompletion();

        
        UIManager.Instance.UpdateMissionUI(
            currentMission.description,
            Mathf.Min(current, currentMission.requiredAmount),
            currentMission.requiredAmount,
            completed
        );

        // Ýlk kez tamamlandýysa ödül ver
        if (!missionCompleted && completed)
        {
            missionCompleted = true;

            UIManager.Instance.playerMoney += currentMission.reward;
            GameManager.Instance.today.earned += currentMission.reward;

            UIManager.Instance.UpdateMoneyUI();
            UIManager.Instance.ShowPopup("Görev tamamlandý! +" + currentMission.reward + "$");

            //  Geçmiþe ekle
            completedMissions.Add(
                $"Gün {GameManager.Instance.currentDay}: {currentMission.description}  +${currentMission.reward}"
            );

            
            UIManager.Instance.UpdateMissionHistoryUI(completedMissions);
        }
    }


    public void GenerateNewMission()
    {
        List<DailyMission> missions = new List<DailyMission>();

        missions.Add(new DailyMission(
            "Balýk sat",
            () => GameManager.Instance.today.fishSold >= 3,
            () => GameManager.Instance.today.fishSold, 
            3,
            100
        ));

        missions.Add(new DailyMission(
            "Balýk besle",
            () => GameManager.Instance.today.fishFed >= 2,
            () => GameManager.Instance.today.fishFed,
            2,
            75
        ));

        currentMission = missions[UnityEngine.Random.Range(0, missions.Count)];
        missionCompleted = false;

        UIManager.Instance.UpdateMissionUI(
                                            currentMission.description,
                                            currentMission.getProgressFunc(),
                                            currentMission.requiredAmount,
                                            false 
                                        );
    }
}
