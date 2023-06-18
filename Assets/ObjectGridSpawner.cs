using UnityEngine;
using UnityEngine.AI;

public class ObjectGridSpawner : MonoBehaviour
{


    public GameObject prefab; // The prefab of the game object you want to spawn
    public int gridSize = 10; // Number of grids in the larger grid
    public int rows = 5; // Number of rows in each grid
    public int columns = 5; // Number of columns in each grid
    public float spacingX = 1f; // Spacing between objects in the X direction
    public float spacingZ = 1f; // Spacing between objects in the Z direction
    public Vector3 startingPosition; // Starting position of the larger grid (bottom left corner)
    public float emptySpaceProbability = 0.2f; // Probability of a space being empty

    private void Start()
    {
        float gridSpacingX = (rows - 1) * (spacingX + CalculateMaxBoundsSize(prefab.GetComponentsInChildren<Renderer>(), Vector3.right));
        float gridSpacingZ = (columns - 1) * (spacingZ + CalculateMaxBoundsSize(prefab.GetComponentsInChildren<Renderer>(), Vector3.forward));

        for (int i = 0; i < gridSize; i++)
        {
            Vector3 gridOffset = new Vector3(i * (gridSpacingX + spacingX), 0f, 0f);

            for (int j = 0; j < gridSize; j++)
            {
                Vector3 spawnPosition = startingPosition + gridOffset + new Vector3(0f, 0f, j * (gridSpacingZ + spacingZ));

                for (int k = 0; k < rows; k++)
                {
                    for (int l = 0; l < columns; l++)
                    {
                        if (Random.value < emptySpaceProbability)
                        {
                            // Skip this position, don't instantiate a game object
                            continue;
                        }

                        float posX = spawnPosition.x + l * (spacingX + CalculateMaxBoundsSize(prefab.GetComponentsInChildren<Renderer>(), Vector3.right));
                        float posZ = spawnPosition.z + k * (spacingZ + CalculateMaxBoundsSize(prefab.GetComponentsInChildren<Renderer>(), Vector3.forward));
                        Vector3 objectSpawnPosition = new Vector3(posX, spawnPosition.y, posZ);
                        Instantiate(prefab, objectSpawnPosition, Quaternion.identity);
                    }
                }
            }
        }

        GenerateNavMesh();
    }

    private float CalculateMaxBoundsSize(Renderer[] renderers, Vector3 direction)
    {
        float maxBoundsSize = 0f;
        foreach (Renderer renderer in renderers)
        {
            float boundsSize = Vector3.Dot(renderer.bounds.size, direction);
            if (boundsSize > maxBoundsSize)
            {
                maxBoundsSize = boundsSize;
            }
        }
        return maxBoundsSize;
    }

    private void GenerateNavMesh()
    {

    }
}
