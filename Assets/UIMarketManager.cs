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
        foreach (var fish in fishList)
        {
            GameObject card = Instantiate(fishCardPrefab, contentArea);

            card.transform.Find("BalýkAdý").GetComponent<TextMeshProUGUI>().text = fish.fishName;
            card.transform.Find("BalýkFiyatý").GetComponent<TextMeshProUGUI>().text = "$" + fish.price.ToString("F2");

            Button buyBtn = card.transform.Find("buyButton").GetComponent<Button>();
            buyBtn.onClick.AddListener(() => TryBuyFish(fish));
        }
    }

    void TryBuyFish(MarketFish fish)
    {
        if (uiManager.playerMoney >= fish.price)
        {
            uiManager.playerMoney -= fish.price;
            uiManager.UpdateMoneyUI();
            Vector3 spawnPos = fishSpawnPoint.position + new Vector3(Random.Range(-1f, 1f), 0, 0);
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
