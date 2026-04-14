using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    public float spawnDelay = 2f;      // oyun bası ilk musteri spawnı
    public float spawnInterval = 5f;   // kac sn de bir musteri

    void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnDelay, spawnInterval);
    }

    void Spawn()
    {
        Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
    }
}

// Erenkaragozz's custom script for [*****]