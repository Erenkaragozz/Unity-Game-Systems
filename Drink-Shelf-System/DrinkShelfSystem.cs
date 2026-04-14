using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkShelfSystem : MonoBehaviour, IInteractable
{
    private Transform playerCamera;
    
    [Header("Raf Ayarları")]
    public Transform[] shelfSpots;
    public float shelfInteractDistance = 2f;
    public KeyCode removeKey = KeyCode.R;

    private AlmaBirakmaSistemi almaScript;

    void Start()
    {
        almaScript = FindFirstObjectByType<AlmaBirakmaSistemi>();
        playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // R ye basınca geri alacak ve adece rafı goruyorsa gerı alma olacak
        if (Input.GetKeyDown(removeKey) && IsLookingAtShelf())
        {
            RevertSpecificDrink();
        }
    }

    public void Interact()
    {
        if (almaScript != null && almaScript.tutulanObje != null)
        {
            DrinkBox box = almaScript.tutulanObje.GetComponent<DrinkBox>();

            if (box != null && box.HasDrink())
            {
                PlaceSpecificDrink(box);
            }
        }
    }

    void PlaceSpecificDrink(DrinkBox box)
    {
        for (int i = 0; i < shelfSpots.Length; i++)
        {
            Transform spot = shelfSpots[i];

            if (spot.childCount == 0)
            {
                GameObject drinkToPlace = box.drinkPrefabInside;
                GameObject newDrink = Instantiate(drinkToPlace, spot.position, spot.rotation);
                newDrink.transform.SetParent(spot);
                
                
                // icecege o anki kolinin ID'sini vediyorz
                DrinkIdentity id = newDrink.AddComponent<DrinkIdentity>();
                id.drinkID = box.boxID; 

                box.UseDrink();
                Debug.Log(box.boxID + " dizildi. Kalan: " + box.currentCount);
                break;
            }
        }
    }

    void RevertSpecificDrink()
    {
        // elimizde bisey yoksa ve koli degilse islem yok
        if (almaScript.tutulanObje == null) return;
        DrinkBox box = almaScript.tutulanObje.GetComponent<DrinkBox>();
        if (box == null) return;

        // raftaki noktaları sondan basa kontrol etme son eklenen ıcecekten baslayıp gerı almak ıcın
        for (int i = shelfSpots.Length - 1; i >= 0; i--)
        {
            Transform spot = shelfSpots[i];
            if (spot.childCount > 0)
            {
                // raftaki icecek kimlik kontrolu
                DrinkIdentity itemID = spot.GetChild(0).GetComponent<DrinkIdentity>();

                // eger raftaki icecek elimizdeki koliyle aynı türdeyse
                if (itemID != null && itemID.drinkID == box.boxID)
                {
                    Destroy(spot.GetChild(0).gameObject);
                    box.AddDrinkBack(); // koliye sayiyi geri ekle
                    Debug.Log("İçecek koliye geri alındı. Yeni sayı: " + box.currentCount);
                    return;
                }
            }
        }
    }

    bool IsLookingAtShelf()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, shelfInteractDistance))
        {
            if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform)) return true;
        }
        return false;
    }

    public string GetInteractionText()
    {
        return "E-Diz / R-Geri Al";
    }
}

// iceceklerin hangiş koliye aitligini anlamak için class
public class DrinkIdentity : MonoBehaviour 
{
    public string drinkID;
}