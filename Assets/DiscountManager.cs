using UnityEngine;
using System.Collections.Generic;

public static class DiscountManager
{
    const string DiscountKey = "DailyDiscountFish";
    const string DiscountDayKey = "Discount_GameDay";

    public static string GetTodayDiscountFish(List<UIMarketManager.MarketFish> fishList, int currentGameDay)
    {
        int savedDay = PlayerPrefs.GetInt(DiscountDayKey, -1);

        if (savedDay != currentGameDay)
        {
            int randomIndex = Random.Range(0, fishList.Count);
            string selectedFish = fishList[randomIndex].fishName;

            PlayerPrefs.SetString(DiscountKey, selectedFish);
            PlayerPrefs.SetInt(DiscountDayKey, currentGameDay);
            PlayerPrefs.Save();

            Debug.Log($"[Ýndirim Güncellendi] {selectedFish} balýðý indirimde | Oyun Günü: {currentGameDay}");
            return selectedFish;
        }

        return PlayerPrefs.GetString(DiscountKey, "");
    }

    public static bool IsFishDiscounted(string fishName)
    {
        return fishName == PlayerPrefs.GetString(DiscountKey, "");
    }
}
