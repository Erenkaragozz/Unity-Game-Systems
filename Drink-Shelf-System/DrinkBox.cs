using UnityEngine;

public class DrinkBox : MonoBehaviour
{
    [Header("Koli İçeriği")]
    public GameObject drinkPrefabInside; // Bu koliden hangi içecek çıkacak?
    public int currentCount = 12;        // Kolide kaç tane var?

    public bool HasDrink()
    {
        return currentCount > 0;
    }

    public void UseDrink()
    {
        currentCount--;
    }
}