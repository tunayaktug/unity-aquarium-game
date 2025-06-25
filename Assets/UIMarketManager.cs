using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIMarketManager : MonoBehaviour
{
    [System.Serializable]
    public class MarketFish
    {
        public string fishName;
        public GameObject fishPrefab;
        public float price;
    }

    public List<MarketFish> fishList; // balýk listesi
    public GameObject fishCardPrefab;
    public Transform contentArea;
    public Transform fishSpawnPoint;
    public UIManager uiManager;
    public GameObject marketPanel;
   

    void Start()
    {
        PopulateMarket();
    }

    void PopulateMarket()
    {
        // Temizle
        foreach (Transform child in contentArea)
            Destroy(child.gameObject);
        foreach (var fish in fishList)
        {
            MarketFish tempFish = fish;  // Fix: closure için
            GameObject card = Instantiate(fishCardPrefab, contentArea);

            card.transform.Find("BalýkAdý").GetComponent<TextMeshProUGUI>().text = tempFish.fishName;
            card.transform.Find("BalýkFiyatý").GetComponent<TextMeshProUGUI>().text = "$" + tempFish.price.ToString("F2");

            Button buyBtn = card.transform.Find("buyButton").GetComponent<Button>();
            buyBtn.onClick.AddListener(() => TryBuyFish(tempFish));  // closure artýk güvenli
        }
    }

    void TryBuyFish(MarketFish fish)
    {
        float price = fish.price;

        if (uiManager.playerMoney >= price)
        {
            uiManager.playerMoney -= price;
            GameManager.Instance.totalSpent += price;
            GameManager.Instance.today.spent += price;

            Debug.Log($"[SATIN ALMA] {fish.fishName} alýndý, fiyat: {price}, kalan para: {uiManager.playerMoney}");

            uiManager.UpdateMoneyUI();

            Vector3 spawnPos = fishSpawnPoint.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, 0);
            Instantiate(fish.fishPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("Yetersiz para!");
        }
    }


    public void ToggleMarket()
    {
        if (marketPanel != null)
        {
            marketPanel.SetActive(!marketPanel.activeSelf);
        }
    }
}
