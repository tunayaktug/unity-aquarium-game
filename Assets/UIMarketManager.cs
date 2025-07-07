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

    [System.Serializable]
    public class MarketAccessory
    {
        public string accessoryName;
        public GameObject accessoryPrefab;
        public float price;
    }

    public List<MarketFish> fishList; // balýk listesi
    public GameObject fishCardPrefab;
    public Transform contentArea;
    public Transform fishSpawnPoint;
    public UIManager uiManager;
    public GameObject marketPanel;

    public List<MarketAccessory> accessoryList; // Inspector’dan eklenecek
    public Transform accessorySpawnPoint; // Mouse ile yerleþtirilecek, burasý geçici konum
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

        foreach (var accessory in accessoryList)
        {
            MarketAccessory tempAcc = accessory;
            GameObject card = Instantiate(fishCardPrefab, contentArea);

            card.transform.Find("BalýkAdý").GetComponent<TextMeshProUGUI>().text = tempAcc.accessoryName;
            card.transform.Find("BalýkFiyatý").GetComponent<TextMeshProUGUI>().text = "$" + tempAcc.price.ToString("F2");
            card.transform.Find("bugünÝndirimde").gameObject.SetActive(false);

            Button accBuyBtn = card.transform.Find("buyButton").GetComponent<Button>();
            accBuyBtn.onClick.AddListener(() => TryBuyAccessory(tempAcc));
        }



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
        // Filtre Sistemi (temizlik hýzý azaltýr)
        if (!GameManager.Instance.hasFilterSystem)
        {
            GameObject filterCard = Instantiate(fishCardPrefab, contentArea);
            filterCard.transform.Find("BalýkAdý").GetComponent<TextMeshProUGUI>().text = "Filtre Sistemi";
            filterCard.transform.Find("BalýkFiyatý").GetComponent<TextMeshProUGUI>().text = "$1000";
            filterCard.transform.Find("bugünÝndirimde").gameObject.SetActive(false);

            Button buyBtn = filterCard.transform.Find("buyButton").GetComponent<Button>();
            buyBtn.onClick.AddListener(() =>
            {
                if (uiManager.playerMoney >= 1000f)
                {
                    uiManager.playerMoney -= 1000f;
                    GameManager.Instance.totalSpent += 1000f;
                    GameManager.Instance.today.spent += 1000f;
                    GameManager.Instance.hasFilterSystem = true;
                    uiManager.UpdateMoneyUI();
                    Debug.Log("Filtre Sistemi satýn alýndý! Temizlik yavaþ bozulacak.");
                    RefreshDiscountAndMarket();
                }
                else
                {
                    uiManager.ShowPopup("Yeterli paran yok!");
                }
            });
        }

        // Yem Otomatiði (açlýk yavaþ artar)
        if (!GameManager.Instance.hasAutoFeeder)
        {
            GameObject feederCard = Instantiate(fishCardPrefab, contentArea);
            feederCard.transform.Find("BalýkAdý").GetComponent<TextMeshProUGUI>().text = "Yem Otomatiði";
            feederCard.transform.Find("BalýkFiyatý").GetComponent<TextMeshProUGUI>().text = "$1000";
            feederCard.transform.Find("bugünÝndirimde").gameObject.SetActive(false);

            Button buyBtn = feederCard.transform.Find("buyButton").GetComponent<Button>();
            buyBtn.onClick.AddListener(() =>
            {
                if (uiManager.playerMoney >= 1000f)
                {
                    uiManager.playerMoney -= 1000f;
                    GameManager.Instance.totalSpent += 1000f;
                    GameManager.Instance.today.spent += 1000f;
                    GameManager.Instance.hasAutoFeeder = true;
                    uiManager.UpdateMoneyUI();
                    Debug.Log("Yem Otomatiði satýn alýndý! Balýklar daha yavaþ acýkacak.");
                    RefreshDiscountAndMarket();
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

    void TryBuyAccessory(MarketAccessory accessory)
    {
        if (uiManager.playerMoney >= accessory.price)
        {
            uiManager.playerMoney -= accessory.price;
            GameManager.Instance.totalSpent += accessory.price;
            GameManager.Instance.today.spent += accessory.price;

            uiManager.UpdateMoneyUI();

            AccessoryPlacementManager.Instance.PrepareAccessoryPlacement(accessory.accessoryPrefab);
        }
        else
        {
            uiManager.ShowPopup("Yeterli paran yok!");
        }
    }

}
