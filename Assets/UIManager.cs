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

    private FishInfo currentFish;
    private void Awake()
    {
        Instance = this;
        infoPanel.SetActive(false);
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
