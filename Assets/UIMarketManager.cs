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
        // EKSTRA ÜRÜNLER: Isýtýcý ve Soðutucu (Sadece gerektiðinde göster)
        var temperature = GameManager.Instance.currentWaterTemperature;

        // Isýtýcý göster (soðuk gün + alýnmamýþsa)
        if (temperature == GameManager.WaterTemperature.Low && !GameManager.Instance.hasHeater)
        {
            GameObject heaterCard = Instantiate(fishCardPrefab, contentArea);

            heaterCard.transform.Find("BalýkAdý").GetComponent<TextMeshProUGUI>().text = "Isýtýcý";
            heaterCard.transform.Find("BalýkFiyatý").GetComponent<TextMeshProUGUI>().text = "$500";
            heaterCard.transform.Find("bugünÝndirimde").gameObject.SetActive(false);

            Button buyBtn = heaterCard.transform.Find("buyButton").GetComponent<Button>();
            buyBtn.onClick.AddListener(() =>
            {
                if (uiManager.playerMoney >= 500f)
                {
                    uiManager.playerMoney -= 500f;
                    GameManager.Instance.totalSpent += 500f;
                    GameManager.Instance.today.spent += 500f;

                    GameManager.Instance.hasHeater = true;
                    uiManager.UpdateMoneyUI();
                    Debug.Log(" Isýtýcý satýn alýndý! Artýk soðuk günlerden etkilenmeyeceksiniz. Ürün kalýcýdýr.");
                    RefreshDiscountAndMarket(); // kartý kaldýrmak için yeniden yükle
                }
                else
                {
                    uiManager.ShowPopup("Yeterli paran yok!");
                }
            });
        }

        // Soðutucu göster (sýcak gün + alýnmamýþsa)
        if (temperature == GameManager.WaterTemperature.High && !GameManager.Instance.hasCooler)
        {
            GameObject coolerCard = Instantiate(fishCardPrefab, contentArea);

            coolerCard.transform.Find("BalýkAdý").GetComponent<TextMeshProUGUI>().text = "Soðutucu";
            coolerCard.transform.Find("BalýkFiyatý").GetComponent<TextMeshProUGUI>().text = "$500";
            coolerCard.transform.Find("bugünÝndirimde").gameObject.SetActive(false);

            Button buyBtn = coolerCard.transform.Find("buyButton").GetComponent<Button>();
            buyBtn.onClick.AddListener(() =>
            {
                if (uiManager.playerMoney >= 500f)
                {
                    uiManager.playerMoney -= 500f;
                    GameManager.Instance.totalSpent += 500f;
                    GameManager.Instance.today.spent += 500f;

                    GameManager.Instance.hasCooler = true;
                    uiManager.UpdateMoneyUI();
                    Debug.Log(" Soðutucu satýn alýndý! Artýk sýcak günlerden etkilenmeyeceksiniz. Ürün kalýcýdýr.");
                    RefreshDiscountAndMarket(); // kartý kaldýrmak için yeniden yükle
                }
                else
                {
                    uiManager.ShowPopup("Yeterli paran yok!");
                }
            });
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
