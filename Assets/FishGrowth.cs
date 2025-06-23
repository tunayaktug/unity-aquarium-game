using UnityEngine;

public class FishGrowth : MonoBehaviour
{
    public float growthInterval = 15f;      // Kaç saniyede bir büyüyecek
    public float growthAmount = 0.01f;
    public float maxScale = 0.2f;
    public float minScale = 0.04f;

    public GameObject growthEffectPrefab;   // Epic Toon FX efekt prefab'ý

    private float timer = 0f;
    private FishInfo fishInfo;

    void Start()
    {
        fishInfo = GetComponent<FishInfo>();
        transform.localScale = Vector3.one * minScale;
    }

    void Update()
    {
        if (fishInfo == null) return;

        timer += Time.deltaTime;

        if (timer >= growthInterval)
        {
            timer = 0f;

            if (fishInfo.hunger < 80f)
            {
                float currentScale = transform.localScale.x;
                if (currentScale < maxScale)
                {
                    float newScale = currentScale + growthAmount;
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
