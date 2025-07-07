using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI dayText;
    public GameObject fishInfoPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI priceText;
    public Image hungerFill;
    public Image healthFill;
    public Gradient gradient;
    public float playerMoney;
    public TextMeshProUGUI moneyText;
    public Button sellButton;
    public TextMeshProUGUI moneyText2;

    [Header("Daily Profit UI")]
    public GameObject dailyProfitPanel;
    public TextMeshProUGUI earnedText;
    public TextMeshProUGUI spentText;
    public TextMeshProUGUI profitText;

    private FishInfo currentFish;

    public TextMeshProUGUI missionText;


    public GameObject missionHistoryPanel;
    public TextMeshProUGUI missionHistoryText;


    public TextMeshProUGUI temperatureText;

    [Header("Water Temperature Visual Effects")]
    public Image freezeOverlay;
    public Image heatOverlay;
    private void Awake()
    {
        Instance = this;
        fishInfoPanel.SetActive(false);

        UpdateMoneyUI();
        sellButton.onClick.AddListener(SellCurrentFish);

        dailyProfitPanel.SetActive(false);
        
    }

    public void UpdateMoneyUI()
    {
        moneyText.text = "Para: $" + playerMoney.ToString("F2");
        moneyText2.text = "Para: $" + playerMoney.ToString("F2");
    }

    public void SellCurrentFish()
    {
        
        if (currentFish != null)
        {
            float salePrice = currentFish.currentPrice;
            playerMoney += currentFish.currentPrice;

            GameManager.Instance.totalEarned += salePrice;
            GameManager.Instance.today.earned += salePrice;

            Destroy(currentFish.gameObject);
            HideInfoPanel();
            UpdateMoneyUI();

            GameManager.Instance.today.fishSold++; 
        }


    }

    public void ShowFishInfo(FishInfo fish)
    {
        if (fish==null)
        {
            return;
        }
 
            currentFish = fish;
            fishInfoPanel.SetActive(true);
            Debug.Log("Yeni balýk bilgisi gösteriliyor: " + fish.fishName);
            nameText.text = "Ýsim: " + currentFish.fishName;
            hungerText.text = "Açlýk: " + currentFish.hunger.ToString("F1");
            healthText.text = "Can: " + currentFish.health.ToString("F1");
            priceText.text = "Fiyat: $" + currentFish.currentPrice.ToString("F2");
        
       
    }

    public void UpdateTemperatureUI()
    {
        string tempText = "";

        switch (GameManager.Instance.currentWaterTemperature)
        {
            case GameManager.WaterTemperature.Normal:
                tempText = "Normal";
                SetTemperatureVisuals(normal: true);
                break;

            case GameManager.WaterTemperature.Low:
                tempText = GameManager.Instance.hasHeater ? "Soðuk (Isýtýcý Aktif)" : "Soðuk ";
                SetTemperatureVisuals(cold: true, heaterActive: GameManager.Instance.hasHeater);

                if (!GameManager.Instance.hasHeater)
                    Debug.Log(" Su çok soðuk! Balýklar yavaþ büyüyecek. Marketten Isýtýcý almanýz gerekiyor.");
                else
                    Debug.Log("Soðuk gün – Isýtýcý aktif, büyüme normal.");
                break;

            case GameManager.WaterTemperature.High:
                tempText = GameManager.Instance.hasCooler ? "Sýcak (Soðutucu Aktif)" : "Sýcak ";
                SetTemperatureVisuals(hot: true, coolerActive: GameManager.Instance.hasCooler);

                if (!GameManager.Instance.hasCooler)
                    Debug.Log(" Su çok sýcak! Balýklar yavaþ büyüyecek. Marketten Soðutucu almanýz gerekiyor.");
                else
                    Debug.Log("Sýcak gün – Soðutucu aktif, büyüme normal.");
                break;
        }

        temperatureText.text = "Su Sýcaklýðý: " + tempText;
    }

    private void SetTemperatureVisuals(bool normal = false, bool cold = false, bool hot = false, bool heaterActive = false, bool coolerActive = false)
    {
        if (cold && !heaterActive)
        {
            freezeOverlay.gameObject.SetActive(true);
        }
        else
        {
            freezeOverlay.gameObject.SetActive(false);
        }

        if (hot && !coolerActive)
        {
            heatOverlay.gameObject.SetActive(true);
        }
        else
        {
            heatOverlay.gameObject.SetActive(false);
        }

        if (normal)
        {
            freezeOverlay.gameObject.SetActive(false);
            heatOverlay.gameObject.SetActive(false);
        }
    }



    void Update()
    {


        if (fishInfoPanel.activeSelf && currentFish != null)
        {

            hungerFill.fillAmount = currentFish.hunger / 100;
            hungerText.text = "Açlýk: " + currentFish.hunger.ToString("F1");

            healthFill.fillAmount = currentFish.health / 100;
            healthText.text = "Can: " + currentFish.health.ToString("F1");

            priceText.text = "Fiyat: $" + currentFish.currentPrice.ToString("F2");

           
            Color hungerColor = gradient.Evaluate(1f - currentFish.hunger / 100f);
            hungerFill.color = hungerColor;

            Color healthColor = gradient.Evaluate(currentFish.health / 100f);
            healthFill.color = healthColor;

            float currentScale = currentFish.transform.localScale.x;
            if (currentScale >= currentFish.minScale * 5f)
            {
                nameText.text = currentFish.fishName + " (Max Seviye)";
            }
            else
            {
                nameText.text = "Ýsim: " + currentFish.fishName;
            }
        }

      
        UpdateDayUI();
        UpdateProfitUI();
    }

    public void UpdateDayUI()
    {
        dayText.text = $"Gün {GameManager.Instance.currentDay}";
    }

    public void UpdateProfitUI()
    {
        var today = GameManager.Instance.today;
        earnedText.text = $"Bugün Kazanýlan: ${today.earned:F2}";
        spentText.text = $"Bugün Harcanan: ${today.spent:F2}";
        profitText.text = $"Bugünün Kârý: ${today.Net:F2}";
    }
    public void HideInfoPanel()
    {
        fishInfoPanel.SetActive(false);
        currentFish = null;
    }

    public void ToggleProfitPanel()
    {
        dailyProfitPanel.SetActive(!dailyProfitPanel.activeSelf);
    }

    public bool IsPointerOverUIObject()
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void UpdateMissionUI(string missionDesc, int current, int target, bool completed)
    {
        if (missionText != null)
        {
            if (completed)
            {
                missionText.text = $"Günlük Görev : {missionDesc} ({target}/{target})";
            }
            else
            {
                missionText.text = $"Günlük Görev: {missionDesc} ({current}/{target})";
            }
        }
    }


    public void ShowPopup(string message)
    {
        Debug.Log(message);
        
    }

    public void UpdateMissionHistoryUI(List<string> missions)
    {
        if (missionHistoryText == null) return;

        missionHistoryText.text = ""; // önce temizle

        foreach (var entry in missions)
        {
            missionHistoryText.text += entry + "\n";
        }
    }

    public void ToggleMissionHistory()
    {
        if (missionHistoryPanel != null)
            missionHistoryPanel.SetActive(!missionHistoryPanel.activeSelf);
    }


}
