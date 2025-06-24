using UnityEngine;

public class FishGrowth : MonoBehaviour
{
    public float growthInterval = 15f;     
    public float growthAmount = 0.01f;
    public float maxScale = 0.2f;
    public float minScale = 0.04f;

    public GameObject growthEffectPrefab;   

    private float timer = 0f;
    private FishInfo fishInfo;

    public AquariumManager aquariumManager;
    private bool hasShownDirtyWarning = false;

    void Start()
    {
        fishInfo = GetComponent<FishInfo>();
        transform.localScale = Vector3.one * minScale;

        if (aquariumManager == null)
        {
            aquariumManager = Object.FindFirstObjectByType<AquariumManager>();
        }
    }

    void Update()
    {
        if (fishInfo == null || aquariumManager == null) return;

        if (aquariumManager.cleanliness <= 0f && !hasShownDirtyWarning)
        {
            Debug.Log("Akvaryum %100 kirli, balýklar büyümüyor! Akvaryumunu hep temizle");
            hasShownDirtyWarning = true;
        }

        if (aquariumManager.cleanliness > 0f && hasShownDirtyWarning)
        {
            hasShownDirtyWarning = false;
        }

        timer += Time.deltaTime;

        if (timer >= growthInterval)
        {
            timer = 0f;

            if (fishInfo.hunger < 80f)
            {
                float cleanlinessMultiplier = aquariumManager.cleanliness / 100f;
                float adjustedGrowthAmount = growthAmount * cleanlinessMultiplier;

                float currentScale = transform.localScale.x;
                if (currentScale < maxScale)
                {
                    float newScale = currentScale + adjustedGrowthAmount;
                    newScale = Mathf.Clamp(newScale, minScale, maxScale);
                    transform.localScale = new Vector3(newScale, newScale, newScale);

                    SpawnGrowthEffect();
                }
            }
        }
    }

    void SpawnGrowthEffect()
    {
        if (growthEffectPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.1f;
            Instantiate(growthEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
