using UnityEngine;

public class FishInfo : MonoBehaviour
{
    public string fishName;
    public float hunger = 0f;
    public float hungerIncreaseRate = 1f;
    public float health = 100f;

    public float basePrice = 30f; 
    public float currentPrice { get; private set; }

    public float minScale = 0.04f;
    public float healthDecreaseRate = 5f; 
    

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
}
