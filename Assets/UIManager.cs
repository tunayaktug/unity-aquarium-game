using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject infoPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI priceText;
    public Slider hungerSlider;
    public Slider healthSlider;
    public float playerMoney = 0f;
    public TextMeshProUGUI moneyText;
    public Button sellButton;


    private FishInfo currentFish;
    private void Awake()
    {
        Instance = this;
        infoPanel.SetActive(false);

        UpdateMoneyUI();
        sellButton.onClick.AddListener(SellCurrentFish);
    }

    void UpdateMoneyUI()
    {
        moneyText.text = "Para: $" + playerMoney.ToString("F2");
    }

    public void SellCurrentFish()
    {
        if (currentFish != null)
        {
            playerMoney += currentFish.currentPrice;
            Destroy(currentFish.gameObject);
            HideInfoPanel();
            UpdateMoneyUI();
        }
    }
    public void ShowFishInfo(FishInfo fish)
    {
        currentFish = fish;


        healthSlider.value = fish.health;
        infoPanel.SetActive(true);
        nameText.text = "Ýsim: " + fish.fishName;
        hungerText.text = "Açlýk: " + fish.hunger.ToString("F1");
        healthText.text = "Can: " + fish.health.ToString("F1");
        priceText.text = "Fiyat: $" + fish.currentPrice.ToString("F2");
        hungerSlider.value = fish.hunger;
    }


    void Update()
    {

        if (infoPanel.activeSelf && currentFish != null)
        {
            hungerSlider.value = currentFish.hunger;
            hungerText.text = "Açlýk: " + currentFish.hunger.ToString("F1");
            healthSlider.value = currentFish.health;
            healthText.text = "Can: " + currentFish.health.ToString("F1");
            priceText.text = "Fiyat: $" + currentFish.currentPrice.ToString("F2");

            //  Max seviye kontrolü
            float currentScale = currentFish.transform.localScale.x;
            if (currentScale >= currentFish.minScale * 5f) // 0.04 * 5 = 0.2
            {
                nameText.text = currentFish.fishName + " (Max Seviye)";
            }
            else
            {
                nameText.text = "Ýsim: " + currentFish.fishName;
            }
        }

        if (infoPanel.activeSelf && currentFish != null)
        {
            healthSlider.value = currentFish.health;
            hungerSlider.value = currentFish.hunger;
            hungerText.text = "Açlýk: " + currentFish.hunger.ToString("F1");
            priceText.text = "Fiyat: $" + currentFish.currentPrice.ToString("F2");
            healthText.text = "Can: " + currentFish.health.ToString("F1");
        }
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
        currentFish = null;
    }

    
}
