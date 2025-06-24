using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.EnhancedTouch;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI dayText;
    public GameObject infoPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI priceText;
    public Image hungerFill;
    public Image healthFill;
    public Gradient gradient;
    public float playerMoney = 0f;
    public TextMeshProUGUI moneyText;
    public Button sellButton;
    public TextMeshProUGUI moneyText2;

    [Header("Daily Profit UI")]
    public GameObject dailyProfitPanel;
    public TextMeshProUGUI earnedText;
    public TextMeshProUGUI spentText;
    public TextMeshProUGUI profitText;

    private FishInfo currentFish;
    private void Awake()
    {
        Instance = this;
        infoPanel.SetActive(false);

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
        float salePrice = currentFish.currentPrice;
        if (currentFish != null)
        {
            playerMoney += currentFish.currentPrice;

            GameManager.Instance.totalEarned += salePrice;
            GameManager.Instance.today.earned += salePrice;

            Destroy(currentFish.gameObject);
            HideInfoPanel();
            UpdateMoneyUI();
        }
    }
    public void ShowFishInfo(FishInfo fish)
    {
        currentFish = fish;

        
       
        infoPanel.SetActive(true);
        nameText.text = "Ýsim: " + fish.fishName;
        hungerText.text = "Açlýk: " + fish.hunger.ToString("F1");
        healthText.text = "Can: " + fish.health.ToString("F1");
        priceText.text = "Fiyat: $" + fish.currentPrice.ToString("F2");
        
    }


    void Update()
    {


        if (infoPanel.activeSelf && currentFish != null)
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
        dayText.text = $"Day {GameManager.Instance.currentDay}";
    }

    public void UpdateProfitUI()
    {
        var today = GameManager.Instance.today;
        earnedText.text = $"Today's Earned: ${today.earned:F2}";
        spentText.text = $"Today's Spent: ${today.spent:F2}";
        profitText.text = $"Today's Profit: ${today.Net:F2}";
    }
    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
        currentFish = null;
    }

    public void ToggleProfitPanel()
    {
        dailyProfitPanel.SetActive(!dailyProfitPanel.activeSelf);
    }


}
