using System;

public class DailyMission
{
    public string description;
    public Func<bool> checkCompletion;
    public float reward;

    public int currentProgress = 0;
    public int requiredAmount = 1;
    public Func<int> getProgressFunc; 

    public DailyMission(string desc, Func<bool> check, Func<int> progress, int required, float rewardAmount)
    {
        description = desc;
        checkCompletion = check;
        getProgressFunc = progress;
        requiredAmount = required;
        reward = rewardAmount;
    }
}
