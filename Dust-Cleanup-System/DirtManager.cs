using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class DirtManager : MonoBehaviour
{
    [Header(" Options ")]
    public GameObject dirtPrefab; 
    public Collider spawnArea;    
    public float spawnInterval = 5f; 
    public int maxDirtCount = 10;
    public float spawnHeightOffset = 0.05f; 

    [Header(" UI ")]
    public TextMeshProUGUI hygieneText; 

    public List<GameObject> currentDirts = new List<GameObject>();
    public float currentShopHygiene = 100f;
    private float timer;

    void Start()
    {
        timer = spawnInterval;
        UpdateUI();
    }

    void Update()
    {
        // zamanlama ve kir spawni
        if (currentDirts.Count < maxDirtCount)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SpawnDirt();
                timer = spawnInterval; 
            }
        }
        CalculateHygiene();
    }

    void SpawnDirt()
    {
        if (dirtPrefab == null || spawnArea == null) return;

        Bounds bounds = spawnArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        
        Vector3 spawnPos = new Vector3(randomX, bounds.center.y + spawnHeightOffset, randomZ);

        // prefabi orjinal durusuyla olusturmak icin dik durma sorunu olmasin diye
        GameObject newDirt = Instantiate(dirtPrefab, spawnPos, Quaternion.Euler(0,0,0));
        
        currentDirts.Add(newDirt);
    }

    // animasyonsuz kucultme yöntemi ile yok etme 
    // kucultme islemini BroomSostem yapacak, en son burayi cagiracak
    public void RemoveDirt(GameObject dirtToRemove)
    {
        if (spawnArea != null && dirtToRemove == spawnArea.gameObject) return;

        if (currentDirts.Contains(dirtToRemove))
        {
            currentDirts.Remove(dirtToRemove);
            Destroy(dirtToRemove);
            CalculateHygiene();
        }
    }
    // ui kisimlari icin 
    void CalculateHygiene()
    {
        float penalty = currentDirts.Count * 10f;
        currentShopHygiene = Mathf.Clamp(100f - penalty, 0f, 100f);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hygieneText != null)
        {
            hygieneText.text = "Hijyen: %" + currentShopHygiene.ToString("F0");
            if (currentShopHygiene > 70) hygieneText.color = Color.green;
            else if (currentShopHygiene < 40) hygieneText.color = Color.red;
            else hygieneText.color = Color.yellow;
        }
    }
}