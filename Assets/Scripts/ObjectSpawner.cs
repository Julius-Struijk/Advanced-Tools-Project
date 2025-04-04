using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] int objectAmount = 5;
    [SerializeField] GameObject spawnObject;
    [SerializeField] GameObject spawnPlatform;
    [SerializeField] float spawnBuffer = 0.2f;
    Vector2 spawnPosition;
    float horizontalPosition = 0;
    float verticalLayer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i <= objectAmount; i++)
        {
            horizontalPosition += spawnBuffer;
            spawnPosition = new (spawnObject.transform.lossyScale.x * horizontalPosition + (spawnPlatform.transform.position.x - spawnPlatform.transform.lossyScale.x / 2), transform.position.y + spawnObject.transform.lossyScale.y * verticalLayer);

            // Checks if the object being spawned is outside of the bounds of the spawn platform.
            if (spawnObject.transform.lossyScale.x * horizontalPosition > spawnPlatform.transform.position.x + spawnPlatform.transform.lossyScale.x )
            {
                // Update position.
                verticalLayer += 1 + spawnBuffer;
                horizontalPosition = spawnBuffer;
                spawnPosition = new (spawnObject.transform.lossyScale.x * horizontalPosition + (spawnPlatform.transform.position.x - spawnPlatform.transform.lossyScale.x / 2) , transform.position.y + spawnObject.transform.lossyScale.y * verticalLayer);
            }

            // Spawn the object
            Instantiate(spawnObject, spawnPosition, transform.rotation, gameObject.transform);
            //Debug.LogFormat("Spawned object {0} at {1} with {2}", i, spawnPosition, horizontalPosition);
            horizontalPosition += 1 + spawnBuffer;
        }
    }
}
