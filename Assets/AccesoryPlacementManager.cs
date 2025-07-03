using UnityEngine;

public class AccessoryPlacementManager : MonoBehaviour
{
    public static AccessoryPlacementManager Instance;

    private GameObject accessoryToPlace;

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

            if (Input.GetMouseButtonDown(0)) // Sol týkla yerleþtir
            {
                accessoryToPlace = null;
               
            }
        }
    }

    public void PrepareAccessoryPlacement(GameObject prefab)
    {
        accessoryToPlace = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
