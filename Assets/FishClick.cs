using UnityEngine;

public class FishClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        FishInfo info = GetComponent<FishInfo>();
        if (info != null)
        {
            UIManager.Instance.ShowFishInfo(info);
        }
    }


    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            FeedFish();
        }
    }

    public GameObject bubbleEffectPrefab; 

    void SpawnBubbleEffect()
    {
        if (bubbleEffectPrefab != null)
        {
            Instantiate(bubbleEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    void FeedFish()
    {
        FishInfo info = GetComponent<FishInfo>();
        if (info != null)
        {
            info.hunger = 0f; 
            SpawnBubbleEffect();
        }
    }

}
