using UnityEngine;

public class FishInfo : MonoBehaviour
{
    public enum FishType
    {
        Normal,
        Piranha
    }
    public string fishName;
    public float hunger = 0f;
    public float hungerIncreaseRate = 1f;
    public float health = 100f;

    public float basePrice = 30f; 
    public float currentPrice { get; private set; }

    public float minScale = 0.04f;
    public float healthDecreaseRate = 5f;

    public FishType fishType = FishType.Normal;
    public Animator animator;
    public bool isDead = false;
    void Update()
    {
        if (isDead) return;

        hunger += hungerIncreaseRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, 100f);
       

       
        if (hunger >= 100f)
        {
            health -= healthDecreaseRate * Time.deltaTime;
            health = Mathf.Clamp(health, 0f, 100f);
         
        }

       
        if (health <= 0f)
        {
            Die();
        }


        float scale = transform.localScale.x;
        currentPrice = basePrice * (scale / minScale);

        if (fishType == FishType.Piranha)
        {
            AttackNearbyFish();
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die"); 
        }

        
        Destroy(gameObject, 2f); 
    }

    void AttackNearbyFish()
    {
        float attackRange = 0.2f;
        float damagePerSecond = 20f;
        float moveSpeed = 1.5f;

        FishInfo[] allFish = FindObjectsByType<FishInfo>(FindObjectsSortMode.None);

        FishInfo closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (FishInfo otherFish in allFish)
        {
            if (otherFish == this) continue;
            if (otherFish.fishType == FishType.Piranha) continue;
            if (otherFish.isDead) continue;

            float distance = Vector3.Distance(transform.position, otherFish.transform.position);
            if (distance < closestDistance)
            {
                closestTarget = otherFish;
                closestDistance = distance;
            }
        }

        if (closestTarget != null)
        {
            float distance = Vector3.Distance(transform.position, closestTarget.transform.position);

            // Yakýnda deðilse, yüzerek yaklaþ
            if (distance > attackRange)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    closestTarget.transform.position,
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                // Yakýndaysa saldýr
                closestTarget.health -= damagePerSecond * Time.deltaTime;

                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }

                if (closestTarget.health <= 0f && !closestTarget.isDead)
                {
                    closestTarget.health = 0f;
                    closestTarget.SendMessage("Die");
                }
            }
        }
    }



}
