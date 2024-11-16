using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer;

    [System.Serializable]
    public class IngredientArea
    {
        public Ingredient ingredientData; // Reference to the ScriptableObject containing ingredient data
        public int ingredientCount = 10; // Number of ingredients to spawn
        public Collider areaCollider; // Reference to the area collider
        public float yOffset = 1f; // Custom y-offset for spawning ingredients above ground
    }

    public List<IngredientArea> ingredientAreas; // List of ingredient areas
    public float spawnRadius = 0.5f; // Radius to check for overlaps (adjust based on prefab size)
    public LayerMask ingredientLayerMask; // Layer to detect ingredients to prevent overlaps
    public int maxAttempts = 50; // Limit the number of attempts for valid points

    void Start()
    {
        SpawnIngredientsInAllAreas();
    }

    void SpawnIngredientsInAllAreas()
    {
        foreach (IngredientArea area in ingredientAreas)
        {
            int spawnedCount = 0;
            int attempts = 0;

            while (spawnedCount < area.ingredientCount && attempts < maxAttempts)
            {
                Vector3 spawnPosition = GetRandomPointInCollider(area.areaCollider);

                // Use raycast to align with terrain
                spawnPosition = AdjustSpawnToTerrain(spawnPosition, area.yOffset);

                // Check for overlaps with existing ingredients
                if (!Physics.CheckSphere(spawnPosition, spawnRadius, ingredientLayerMask))
                {
                    // Instantiate the prefab from the ScriptableObject and set as child of the area collider
                    GameObject ingredientInstance = Instantiate(area.ingredientData.prefab, spawnPosition, Quaternion.identity);
                    ingredientInstance.transform.SetParent(area.areaCollider.transform);
                    spawnedCount++;
                }
                attempts++;
            }

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning($"Failed to spawn all ingredients in area {area.areaCollider.name}. Spawned {spawnedCount}/{area.ingredientCount}.");
            }
        }
    }

    Vector3 GetRandomPointInCollider(Collider areaCollider)
    {
        return new Vector3(
            Random.Range(areaCollider.bounds.min.x, areaCollider.bounds.max.x),
            areaCollider.bounds.max.y + 5f, // Start above the collider
            Random.Range(areaCollider.bounds.min.z, areaCollider.bounds.max.z)
        );
    }

    Vector3 AdjustSpawnToTerrain(Vector3 spawnPosition, float yOffset)
    {
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
        {
            // Align the y-coordinate to the terrain and add the yOffset
            return new Vector3(spawnPosition.x, hit.point.y + yOffset, spawnPosition.z);
        }
        return spawnPosition; // Return the original position if no terrain is hit
    }
}

