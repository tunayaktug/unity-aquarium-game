using UnityEngine;

public class FishClickHandler : MonoBehaviour
{
    public FishInfo fishinfo;
    private UIManager uiManager;
    private void Start()
    {
        uiManager = UIManager.Instance;
    }
    private void OnMouseDown()
    {

        if (uiManager == null)
        {
            return;
        }
        if (!uiManager.IsPointerOverUIObject())
        {
            
            Debug.Log("a");
            uiManager.ShowFishInfo(fishinfo);
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
            GameManager.Instance.today.fishFed++; 

        }
    }

}
