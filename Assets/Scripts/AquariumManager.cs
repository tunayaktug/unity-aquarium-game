using UnityEngine;
using UnityEngine.UI;

public class AquariumManager : MonoBehaviour
{
    public float cleanliness = 100f;
    public float decayRate = 1f;
    public Image dirtyOverlay;

    void Update()
    {
        float effectiveDecay = decayRate;

        if (GameManager.Instance.hasFilterSystem)
            effectiveDecay *= 0.5f; 
        cleanliness -= decayRate * Time.deltaTime;
        cleanliness = Mathf.Clamp(cleanliness, 0f, 100f);

        UpdateDirtyEffect();
    }

    void UpdateDirtyEffect()
    {
        float alpha = Mathf.Lerp(0f, 0.6f, 1f - (cleanliness / 100f));
        Color color = dirtyOverlay.color;
        color.a = alpha;
        dirtyOverlay.color = color;
    }

    public void Clean()
    {
        cleanliness = 100f;
        Debug.Log("Akvaryum temizlendi!");
    }
}
