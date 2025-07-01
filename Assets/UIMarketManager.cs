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
        
        int gameDay = GameManager.Instance.currentDay;
        string todayDiscountFish = DiscountManager.GetTodayDiscountFish(fishList, gameDay);

        Debug.Log("Bugünün (Oyun Günü) Ýndirimli Balýðý: " + todayDiscountFish);
        PopulateMarket();
    }

    void PopulateMarket()
    {
        int gameDay = GameManager.Instance.currentDay;
        string discountFish = DiscountManager.GetTodayDiscountFish(fishList, gameDay);

        foreach (Transform child in contentArea)
            Destroy(child.gameObject);

        foreach (var fish in fishList)
        {
            MarketFish tempFish = fish;
            GameObject card = Instantiate(fishCardPrefab, contentArea);

            // Ýsim ve fiyat
            card.transform.Find("BalýkAdý").GetComponent<TextMeshProUGUI>().text = tempFish.fishName;

            float displayPrice = tempFish.price;
            bool isDiscounted = DiscountManager.IsFishDiscounted(tempFish.fishName);

            if (isDiscounted)
            {
                displayPrice *= 0.5f; // %50 indirim (istersen oraný deðiþtir)
                card.transform.Find("bugünÝndirimde").gameObject.SetActive(true); // UI'de "Ýndirimde" etiketi
            }
            else
            {
                card.transform.Find("bugünÝndirimde").gameObject.SetActive(false);
            }

            card.transform.Find("BalýkFiyatý").GetComponent<TextMeshProUGUI>().text = "$" + displayPrice.ToString("F2");

            Button buyBtn = card.transform.Find("buyButton").GetComponent<Button>();
            buyBtn.onClick.AddListener(() => TryBuyFish(tempFish, displayPrice)); // fiyatý geçiyoruz
        }
    }



    void TryBuyFish(MarketFish fish, float priceToPay)
    {
        if (uiManager.playerMoney >= priceToPay)
        {
            uiManager.playerMoney -= priceToPay;
            GameManager.Instance.totalSpent += priceToPay;
            GameManager.Instance.today.spent += priceToPay;

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

    public void RefreshDiscountAndMarket()
    {
        int gameDay = GameManager.Instance.currentDay;
        string discountFish = DiscountManager.GetTodayDiscountFish(fishList, gameDay);
        Debug.Log("Yeni Gün Yeni indirimli balýk: " + discountFish);
        PopulateMarket();
    }

}
