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

    public List<MarketFish> fishList; // bal�k listesi
    public GameObject fishCardPrefab;
    public Transform contentArea;
    public Transform fishSpawnPoint;
    public UIManager uiManager;
    public GameObject marketPanel;
   

    void Start()
    {
        
        int gameDay = GameManager.Instance.currentDay;
        string todayDiscountFish = DiscountManager.GetTodayDiscountFish(fishList, gameDay);

        Debug.Log("Bug�n�n (Oyun G�n�) �ndirimli Bal���: " + todayDiscountFish);
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

            // �sim ve fiyat
            card.transform.Find("Bal�kAd�").GetComponent<TextMeshProUGUI>().text = tempFish.fishName;

            float displayPrice = tempFish.price;
            bool isDiscounted = DiscountManager.IsFishDiscounted(tempFish.fishName);

            if (isDiscounted)
            {
                displayPrice *= 0.5f; // %50 indirim (istersen oran� de�i�tir)
                card.transform.Find("bug�n�ndirimde").gameObject.SetActive(true); // UI'de "�ndirimde" etiketi
            }
            else
            {
                card.transform.Find("bug�n�ndirimde").gameObject.SetActive(false);
            }

            card.transform.Find("Bal�kFiyat�").GetComponent<TextMeshProUGUI>().text = "$" + displayPrice.ToString("F2");

            Button buyBtn = card.transform.Find("buyButton").GetComponent<Button>();
            buyBtn.onClick.AddListener(() => TryBuyFish(tempFish, displayPrice)); // fiyat� ge�iyoruz
        }
        // EKSTRA �R�NLER: Is�t�c� ve So�utucu (Sadece gerekti�inde g�ster)
        var temperature = GameManager.Instance.currentWaterTemperature;

        // Is�t�c� g�ster (so�uk g�n + al�nmam��sa)
        if (temperature == GameManager.WaterTemperature.Low && !GameManager.Instance.hasHeater)
        {
            GameObject heaterCard = Instantiate(fishCardPrefab, contentArea);

            heaterCard.transform.Find("Bal�kAd�").GetComponent<TextMeshProUGUI>().text = "Is�t�c�";
            heaterCard.transform.Find("Bal�kFiyat�").GetComponent<TextMeshProUGUI>().text = "$500";
            heaterCard.transform.Find("bug�n�ndirimde").gameObject.SetActive(false);

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
                    Debug.Log(" Is�t�c� sat�n al�nd�! Art�k so�uk g�nlerden etkilenmeyeceksiniz. �r�n kal�c�d�r.");
                    RefreshDiscountAndMarket(); // kart� kald�rmak i�in yeniden y�kle
                }
                else
                {
                    uiManager.ShowPopup("Yeterli paran yok!");
                }
            });
        }

        // So�utucu g�ster (s�cak g�n + al�nmam��sa)
        if (temperature == GameManager.WaterTemperature.High && !GameManager.Instance.hasCooler)
        {
            GameObject coolerCard = Instantiate(fishCardPrefab, contentArea);

            coolerCard.transform.Find("Bal�kAd�").GetComponent<TextMeshProUGUI>().text = "So�utucu";
            coolerCard.transform.Find("Bal�kFiyat�").GetComponent<TextMeshProUGUI>().text = "$500";
            coolerCard.transform.Find("bug�n�ndirimde").gameObject.SetActive(false);

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
                    Debug.Log(" So�utucu sat�n al�nd�! Art�k s�cak g�nlerden etkilenmeyeceksiniz. �r�n kal�c�d�r.");
                    RefreshDiscountAndMarket(); // kart� kald�rmak i�in yeniden y�kle
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
        Debug.Log("Yeni G�n Yeni indirimli bal�k: " + discountFish);
        PopulateMarket();
    }

}
