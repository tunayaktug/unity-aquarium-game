using UnityEngine;

public class SmartFish2D : MonoBehaviour
{
    public float speed = 2f;
    public float directionChangeInterval = 3f;

    
    private float minX = -6.005f;
    private float maxX = 6.005f;
    private float minY = -1.541f;
    private float maxY = 1.541f;

    private Vector2 swimDirection;
    private float timer;

    void Start()
    {
        ChooseNewDirection();
    }

    void Update()
    {
        
        Vector3 newPosition = transform.position + (Vector3)(swimDirection * speed * Time.deltaTime);
        newPosition.z = 0f; 

    
        if (newPosition.x < minX || newPosition.x > maxX)
        {
            swimDirection.x = -swimDirection.x;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            FlipFish();
        }

        if (newPosition.y < minY || newPosition.y > maxY)
        {
            swimDirection.y = -swimDirection.y;
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }

        transform.position = newPosition;

    
        timer += Time.deltaTime;
        if (timer >= directionChangeInterval)
        {
            ChooseNewDirection();
            timer = 0f;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 1f);
    }

    void ChooseNewDirection()
    {
        float xDir = Random.value < 0.5f ? -1f : 1f;
        float yDir = Random.Range(-0.5f, 0.5f);
        swimDirection = new Vector2(xDir, yDir).normalized;

        FlipFish();
    }

    private Quaternion targetRotation;

    void FlipFish()
    {
        if (swimDirection.x == 0)
            return;

        float yAngle = swimDirection.x > 0 ? 90f : -90f;
        targetRotation = Quaternion.Euler(0f, yAngle, 0f);
    }



}
