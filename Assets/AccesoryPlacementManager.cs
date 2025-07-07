using UnityEngine;

public class AccessoryPlacementManager : MonoBehaviour
{
    public static AccessoryPlacementManager Instance;

    private GameObject accessoryToPlace;
    private int currentRotation = 0; 

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (accessoryToPlace != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // 2D oyun için

            accessoryToPlace.transform.position = mouseWorldPos;

            // E tuþuna basýldýðýnda 90 derece döndür
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentRotation = (currentRotation + 90) % 360;
                accessoryToPlace.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
            }

           
            if (Input.GetMouseButtonDown(0))
            {
                accessoryToPlace = null;
            }
        }
    }

    public void PrepareAccessoryPlacement(GameObject prefab)
    {
        accessoryToPlace = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        currentRotation = 0;
    }
}
