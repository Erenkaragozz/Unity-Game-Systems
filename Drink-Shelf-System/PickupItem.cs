using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName = "Obje"; // �rn: "B��ak", "Tabak"

    public string GetInteractionText()
    {
        return "Al: " + itemName; // Ekranda "E - Al: B��ak" yazar
    }

    public void Interact()
    {
        // Oyundaki Alma sistemini bul
        AlmaBirakmaSistemi almaSistemi = FindFirstObjectByType<AlmaBirakmaSistemi>();

        // E�er elimiz bo�sa bu objeyi al
        if (almaSistemi != null && almaSistemi.tutulanObje == null)
        {
            almaSistemi.Al(this.gameObject);
        }
    }
}